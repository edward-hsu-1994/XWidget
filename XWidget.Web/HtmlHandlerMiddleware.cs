using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace XWidget.Web {
    public static class HtmlHandlerMiddleware {
        /// <summary>
        /// 攔截並處理Html回應
        /// </summary>
        /// <param name="app">應用程式建構器</param>
        /// <param name="handler">處理程序</param>
        /// <returns>應用程式建構器</returns>
        public static IApplicationBuilder UseHtmlHandler(
            this IApplicationBuilder app,
            Func<HttpContext, string, Task<string>> handler) {
            return UseHtmlHandler(app, (c, x) => handler(c, x).GetAwaiter().GetResult());
        }

        /// <summary>
        /// 攔截並處理Html回應
        /// </summary>
        /// <param name="app">應用程式建構器</param>
        /// <param name="handler">處理程序</param>
        /// <returns>應用程式建構器</returns>
        public static IApplicationBuilder UseHtmlHandler(
            this IApplicationBuilder app,
            Func<HttpContext, string, string> handler) {
            return app.Use(async (context, next) => {
                var originStream = context.Response.Body;
                long? originStreamLength = context.Response.ContentLength;

                Stream fakeBody = new MemoryStream();

                context.Response.Body = fakeBody;
                await next();

                if (context.Response.ContentType == "text/html") {
                    fakeBody.Seek(0, SeekOrigin.Begin);

                    // 讀取HTML內容
                    var html = await new StreamReader(fakeBody).ReadToEndAsync();

                    html = handler(context, html);

                    // 字串轉Stream
                    fakeBody = new MemoryStream();
                    StreamWriter streamWriter = new StreamWriter(fakeBody);
                    streamWriter.Write(html);
                    streamWriter.Flush();
                }
                fakeBody.Seek(0, SeekOrigin.Begin);

                context.Response.Body = originStream;
                context.Response.ContentLength = (originStreamLength ?? 0) + fakeBody.Length;

                await fakeBody.CopyToAsync(originStream);
            });
        }
    }
}
