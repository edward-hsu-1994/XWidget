using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace XWidget.Web {
    public static class BasicAuthenticateScopeExtension {
        /// <summary>
        /// 於指定路由下使用基本HTTP驗證
        /// </summary>
        /// <typeparam name="TBaseAuthorizeHandler">驗證處理類型</typeparam>
        /// <param name="app">應用程式建構器</param>
        /// <param name="path">路徑</param>
        /// <param name="loginCond">登入判斷條件</param>
        /// <returns>應用程式建構器</returns>
        public static IApplicationBuilder UseBasicAuthenticateScope<TBaseAuthorizeHandler>(
            this IApplicationBuilder app,
            PathString path,
            BasicAuthorizeFunc loginCond)
            where TBaseAuthorizeHandler : class, IBaseAuthorizeHandler {
            return app.UseMiddleware<BasicAuthenticateScopeMiddleware<TBaseAuthorizeHandler>>(path, loginCond);
        }

        /// <summary>
        /// 加入驗證處理類型
        /// </summary>
        /// <typeparam name="TBaseAuthorizeHandler">驗證處理類型</typeparam>
        /// <param name="services">服務集合</param>
        /// <returns>服務集合</returns>
        public static IServiceCollection AddBasicAuthorizeHandler<TBaseAuthorizeHandler>(
            this IServiceCollection services)
            where TBaseAuthorizeHandler : class, IBaseAuthorizeHandler {
            return services.AddScoped<TBaseAuthorizeHandler>();
        }
    }
}
