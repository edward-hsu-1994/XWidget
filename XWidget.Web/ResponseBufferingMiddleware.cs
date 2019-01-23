using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace XWidget.Web {
    public static class ResponseBufferingMiddleware {
        public static IApplicationBuilder UseResponseBuffering(this IApplicationBuilder app) {
            return app.Use(async (context, next) => {
                var originStream = context.Response.Body;
                var warpStream = new MemoryStream();

                context.Response.Body = warpStream;
                await next();
                warpStream.Seek(0, SeekOrigin.Begin);

                context.Response.Body = originStream;

                #region Backup Response Properties
                var backup = new {
                    context.Response.ContentType,
                    context.Response.StatusCode,
                    Headers = context.Response.Headers.ToArray()
                };
                #endregion

                context.Response.Clear();

                #region Reset Response Properties
                context.Response.ContentType = backup.ContentType;
                context.Response.StatusCode = backup.StatusCode;
                foreach (var header in backup.Headers) {
                    if (header.Key == "Content-Length") continue;
                    context.Response.Headers[header.Key] = header.Value;
                }
                #endregion

                await warpStream.CopyToAsync(context.Response.Body);
                context.Response.ContentLength = warpStream.Length;
            });
        }
    }
}
