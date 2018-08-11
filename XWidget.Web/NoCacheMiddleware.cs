using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection {
    /// <summary>
    /// 針對ASP.net Core提供全域No ResponseCache設定
    /// </summary>
    public static class NoCacheMiddleware {
        /// <summary>
        /// 使用全域No ResponseCache預設設定
        /// </summary>
        /// <param name="app">應用程式建構器</param>
        /// <returns>應用程式建構器</returns>
        public static IApplicationBuilder UseNoCache(this IApplicationBuilder app) {
            return app.Use(async (req, next) => {
                req.Response.Headers["Cache-Control"] = "no-cache, no-store";
                await next.Invoke();
            });
        }
    }
}
