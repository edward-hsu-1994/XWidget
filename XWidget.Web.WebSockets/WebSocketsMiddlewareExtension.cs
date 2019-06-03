using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace XWidget.Web.WebSockets {
    public static class WebSocketsMiddlewareExtension {
        /// <summary>
        /// 將WebSocket服務加入管線流程，使用指定的WebSocketHandler
        /// </summary>
        /// <typeparam name="THandler">處理容器型別，必須繼承自WebSocketHandler</typeparam>
        /// <param name="app">擴充對象</param>
        /// <param name="path">路徑</param>
        /// <param name="options">WebSocket選項</param>
        public static IApplicationBuilder UseWebSockets<THandler>(
            this IApplicationBuilder app,
            PathString path,
            WebSocketOptions options = null)
            where THandler : WebSocketHandlerBase {

            if (options == null) {
                app.UseWebSockets();
            } else {
                app.UseWebSockets(options);
            }

            THandler handler = (THandler)Activator.CreateInstance(typeof(THandler));

            app.Map(path, app2 => {
                app2.UseMiddleware<WebSocketsMiddleware<THandler>>(options);
            });

            return app;
        }
    }
}
