using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace XWidget.Web {
    public delegate bool BasicAuthorizeFunc(string account, string password);

    /// <summary>
    /// 基本HTTP驗證中間層
    /// </summary>
    public class BasicAuthenticateScopeMiddleware {
        private readonly RequestDelegate Next;
        private BasicAuthorizeFunc Authorize { get; set; }
        private PathString Path { get; set; }
        public BasicAuthenticateScopeMiddleware
            (RequestDelegate next,
            PathString path,
            BasicAuthorizeFunc authorize) {
            Next = next;
            Path = path;
            Authorize = authorize;
        }

        public async Task Invoke(HttpContext context) {
            if (context.Request.Path.StartsWithSegments(Path)) {
                if (context.Request.Headers.ContainsKey("Authorization")) {
                    var authData = Encoding.UTF8.GetString(Convert.FromBase64String(context.Request.Headers["Authorization"].ToString().Split(' ')[1])).Split(':');
                    if (Authorize(authData[0], authData[1])) {
                        await Next(context);
                    } else {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "text/plain";
                        await context.Response.WriteAsync("403 Forbidden.");
                    }
                } else {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "text/plain";
                    context.Response.Headers["WWW-Authenticate"] = "Basic realm=\"Developer Only\"";
                    await context.Response.WriteAsync("401 Unauthorized.");
                }
            } else {
                await Next(context);
            }
        }
    }
}
