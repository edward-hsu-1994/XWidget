using XWidget.EFLogic;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection {
    /// <summary>
    /// 邏輯管理器擴充
    /// </summary>
    public static class LogicExtension {
        /// <summary>
        /// 加入邏輯管理
        /// </summary>
        /// <typeparam name="TLogic">邏輯管理器類型</typeparam>
        /// <typeparam name="TContext">資料庫內容類型</typeparam>
        /// <typeparam name="TParameters">傳遞參數類型</typeparam>
        /// <param name="services">服務集合</param>
        /// <param name="optionsAction">EntityFramework選項</param>
        /// <returns>動態邏輯建構器</returns>
        public static DynamicLogicMapBuilder<TContext> AddLogic<TLogic, TContext, TParameters>(
            this IServiceCollection services,
            Action<IServiceProvider, DbContextOptionsBuilder> optionsAction)
            where TLogic : LogicManagerBase<TContext, TParameters>
            where TContext : DbContext {
            services.AddDbContext<TContext>(optionsAction);

            return services.GetDynamicLogicMapBuilder<TLogic, TContext, TParameters>();
        }

        /// <summary>
        /// 加入邏輯管理
        /// </summary>
        /// <typeparam name="TLogic">邏輯管理器類型</typeparam>
        /// <typeparam name="TContext">資料庫內容類型</typeparam>
        /// <typeparam name="TParameters">傳遞參數類型</typeparam>
        /// <param name="services">服務集合</param>
        /// <param name="optionsAction">EntityFramework選項</param>
        /// <returns>動態邏輯建構器</returns>
        public static DynamicLogicMapBuilder<TContext> AddLogic<TLogic, TContext, TParameters>(
            this IServiceCollection services,
            Action<DbContextOptionsBuilder> optionsAction)
            where TLogic : LogicManagerBase<TContext, TParameters>
            where TContext : DbContext {
            services.AddDbContext<TContext>(optionsAction);

            return services.GetDynamicLogicMapBuilder<TLogic, TContext, TParameters>();
        }

        /// <summary>
        /// 加入邏輯管理
        /// </summary>
        /// <typeparam name="TLogic">邏輯管理器類型</typeparam>
        /// <typeparam name="TContext">資料庫內容類型</typeparam>
        /// <typeparam name="TParameters">傳遞參數類型</typeparam>
        /// <param name="services">服務集合</param>
        /// <returns>動態邏輯建構器</returns>
        public static DynamicLogicMapBuilder<TContext> AddLogic<TLogic, TContext, TParameters>(
            this IServiceCollection services)
            where TLogic : LogicManagerBase<TContext, TParameters>
            where TContext : DbContext {
            services.AddDbContext<TContext>();

            return services.GetDynamicLogicMapBuilder<TLogic, TContext, TParameters>();
        }

        private static DynamicLogicMapBuilder<TContext> GetDynamicLogicMapBuilder<TLogic, TContext, TParameters>(
            this IServiceCollection services)
            where TLogic : LogicManagerBase<TContext, TParameters>
            where TContext : DbContext {
            var builder = new DynamicLogicMapBuilder<TContext>();

            services.AddScoped<InternalLogicManagerContainer<TContext, TParameters>>();

            services.AddScoped<TLogic>(serviceProvider => {
                var constructors = typeof(TLogic).GetConstructors().FirstOrDefault();
                var createServices = constructors.GetParameters().Select(x => x.ParameterType)
                    .Select(x => serviceProvider.GetService(x)).ToArray();
                var instance = (TLogic)constructors.Invoke(createServices);
                instance.MapBuilder = builder;
                instance.ServiceProvider = serviceProvider;

                var scopeContainer = serviceProvider.GetService<InternalLogicManagerContainer<TContext, TParameters>>();
                scopeContainer.Manager = instance;

                foreach (var property in typeof(TLogic).GetProperties()) {
                    var isLogic = GetAllBaseTypes(property.PropertyType).Where(x => x.IsGenericType).Select(x => x.GetGenericTypeDefinition()).Contains(typeof(LogicBase<,,,>));

                    if (!isLogic) continue;

                    var propertyValue = serviceProvider.GetService(property.PropertyType);

                    property.SetValue(instance, propertyValue);
                }

                return instance;
            });

            foreach (var property in typeof(TLogic).GetProperties()) {
                var isLogic = GetAllBaseTypes(property.PropertyType).Where(x => x.IsGenericType).Select(x => x.GetGenericTypeDefinition()).Contains(typeof(LogicBase<,,,>));

                if (!isLogic) continue;

                services.AddScoped(property.PropertyType, serviceProvider => {
                    var scopeContainer = serviceProvider.GetService<InternalLogicManagerContainer<TContext, TParameters>>();

                    var constuctor = property.PropertyType.GetConstructors().Single();

                    var parameterValues = new List<object>();
                    foreach (var param in constuctor.GetParameters()) {
                        var isLogicManager = param.ParameterType.GetAllBaseTypes().Where(x => x.IsGenericType).Select(x => x.GetGenericTypeDefinition()).Contains(typeof(LogicManagerBase<,>));

                        if (isLogicManager) {
                            parameterValues.Add(scopeContainer.Manager ?? serviceProvider.GetService(param.ParameterType));
                        } else {
                            parameterValues.Add(serviceProvider.GetService(param.ParameterType));
                        }
                    }

                    return constuctor.Invoke(parameterValues.ToArray());
                });
            }

            return builder;
        }

        private static Type[] GetAllBaseTypes(this Type type) {
            if (type == null) {
                return Type.EmptyTypes;
            }
            if (type == typeof(object)) {
                return new Type[] { typeof(object) };
            }
            return new Type[] { type }.Concat(GetAllBaseTypes(type.BaseType)).ToArray();
        }
    }
}