using Microsoft.AspNetCore.Builder;
using System;

namespace Microsoft.Extensions.DependencyInjection {
    public static class ServiceProviderExtension {
        /// <summary>
        /// DI服務提供者
        /// </summary>
        internal static IServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// 使用本工廠類別於執行階段取得.NET Core DI服務提供者
        /// </summary>
        /// <param name="builder">應用程式建構器</param>
        public static void UseServiceProviderHelper(this IApplicationBuilder builder) {
            ServiceProvider = builder.ApplicationServices;
        }
    }
}
