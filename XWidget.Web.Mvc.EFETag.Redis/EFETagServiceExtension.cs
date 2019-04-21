using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace XWidget.Web.Mvc.EFETag.Redis {
    public static class EFETagServiceExtension {
        internal static IConnectionMultiplexer cm;
        internal static ISubscriber subscriber;
        internal static IDatabase database;

        /// <summary>
        /// 使用Redis進行多機同步
        /// </summary>
        /// <typeparam name="TContext">DbContext類型</typeparam>
        /// <param name="services">服務集合</param>
        /// <param name="connectionMultiplexer">Redis連線</param>
        /// <returns>服務集合</returns>
        public static IServiceCollection AddEFETagRedis<TContext>(
            this IServiceCollection services,
            IConnectionMultiplexer connectionMultiplexer) where TContext : DbContext {
            EFETagServiceExtension.cm = connectionMultiplexer;
            EFETagServiceExtension.subscriber = connectionMultiplexer.GetSubscriber();
            EFETagServiceExtension.database = connectionMultiplexer.GetDatabase();
            subscriber.Subscribe("etags", (channel, message) => {
                var etag = ((string)message).Split('=');
                Console.WriteLine(message);
            });
            subscriber.Subscribe("etags", (channel, message) => {
                var etag = ((string)message).Split('=');
                EFETagMiddleware<TContext>.UpdateETag(etag[0], etag[1], true);
            });
            EFETagMiddleware<TContext>.ETagUpdated += _ETagUpdated;
            EFETagMiddleware<TContext>.ETagInit += _ETagInit<TContext>;
            return services;
        }

        private static void _ETagInit<TContext>(object sender, EventArgs e)
            where TContext : DbContext {
            if (database != null) {
                var extsisEtags = EFETagServiceExtension.database.HashGetAll("etags");
                foreach (var etag in extsisEtags) {
                    EFETagMiddleware<TContext>.ETags[etag.Name] = etag.Value;
                }
                foreach (var etag in EFETagMiddleware<TContext>.ETags) {
                    database.HashSet("etags", etag.Key, etag.Value);
                }
            }
        }

        private static void _ETagUpdated(object sender, KeyValuePair<string, string> e) {
            Task.Run(() => {
                subscriber.Publish("etags", $"{e.Key}={e.Value}");
                database.HashSet("etags", e.Key, e.Value);
            });
        }
    }
}
