using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace XWidget.Web {
    public static class BasicAuthenticateScopeExtension {
        /// <summary>
        /// 使用範圍HTTP基本驗證
        /// </summary>
        /// <typeparam name="TBaseAuthorizeHandler">驗證類別</typeparam>
        /// <param name="app">應用程式建構器</param>
        /// <param name="path">路徑</param>
        /// <param name="options">選項</param>
        /// <returns>應用程式建構器</returns>
        public static IApplicationBuilder UseBasicAuthenticateScope<TBaseAuthorizeHandler>(
            this IApplicationBuilder app,
            PathString path,
            IOptions<BasicAuthenticateScopeOption> options)
            where TBaseAuthorizeHandler : IBaseAuthorizeHandler {
            return app.UseMiddleware<BasicAuthenticateScopeMiddleware<TBaseAuthorizeHandler>>(
                options
            );
        }
    }
}
