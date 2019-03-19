using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace XWidget.Web {
    public static class BasicAuthenticateScopeExtension {
        /// <summary>
        /// 於指定路由下使用基本HTTP驗證
        /// </summary>
        /// <param name="app">應用程式建構器</param>
        /// <param name="path">路徑</param>
        /// <param name="loginCond">登入判斷條件</param>
        /// <returns>應用程式建構器</returns>
        public static IApplicationBuilder UseBasicAuthenticateScope(
            this IApplicationBuilder app,
            PathString path,
            BasicAuthorizeFunc loginCond) {
            return app.UseMiddleware<BasicAuthenticateScopeMiddleware>(path, loginCond);
        }
    }
}
