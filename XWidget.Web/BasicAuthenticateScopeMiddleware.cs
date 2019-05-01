using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace XWidget.Web {
    /// <summary>
    /// 基本HTTP驗證中間層
    /// </summary>
    /// <typeparam name="TBaseAuthorizeHandler">基礎驗證處理類型，為使用者實作的基礎驗證別</typeparam>
    public class BasicAuthenticateScopeMiddleware<TBaseAuthorizeHandler>
        where TBaseAuthorizeHandler : IBaseAuthorizeHandler {
        // HTTP管線下一個步驟
        public RequestDelegate Next { get; set; }

        // 需要基本驗證的路由
        private PathString Path { get; set; }

        // 領域
        private string Realm { get; set; }

        // 建構子，取得下一階段管線流程以及自UseMiddleware方法中使用的路徑以及驗證方法
        public BasicAuthenticateScopeMiddleware(
            RequestDelegate next,
            IOptions<BasicAuthenticateScopeOption> authOption
            ) {
            Next = next;
            Path = authOption.Value.Path;
            Realm = authOption.Value.Realm;
        }

        // 中間層流程
        public async Task InvokeAsync(HttpContext context) {
            // 檢驗目前的Request Path是否為要進行基本驗證的路由
            if (context.Request.Path.StartsWithSegments(Path)) {
                // 檢查是否攜帶驗證標頭
                if (context.Request.Headers.ContainsKey("Authorization")) {
                    // 發現驗證標頭，解析驗證資訊
                    try {
                        var authData = Encoding.UTF8.GetString(Convert.FromBase64String(context.Request.Headers["Authorization"].ToString().Split(' ')[1])).Split(':');
                        // 自DI提供者中取得泛型中指定的基礎驗證處理類別實例
                        var handler = (TBaseAuthorizeHandler)context.RequestServices.GetService(typeof(TBaseAuthorizeHandler));
                        // 調用驗證方法確認驗證是否通過
                        if (await handler.Authorize(authData[0], authData[1])) {
                            // 通過驗證則繼續處理後去動作
                            await Next(context);
                        } else {
                            // 驗證失敗拋出403狀態與錯誤訊息
                            context.Response.StatusCode = StatusCodes.Status403Forbidden;
                            context.Response.ContentType = "text/plain";
                            context.Response.Headers["WWW-Authenticate"] = $"Basic realm=\"{Realm}\"";
                            await context.Response.WriteAsync("401 Unauthorized.");
                        }
                    } catch {
                        // 驗證與剖析過程出現例外，拋回錯誤
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "text/plain";
                        context.Response.Headers["WWW-Authenticate"] = $"Basic realm=\"{Realm}\"";
                        await context.Response.WriteAsync("401 Unauthorized.");
                    }
                } else {
                    // 未攜帶資訊，拋出驗證需求標頭、401狀態以及realm資訊
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "text/plain";
                    context.Response.Headers["WWW-Authenticate"] = $"Basic realm=\"{Realm}\"";
                    await context.Response.WriteAsync("401 Unauthorized.");
                }
            } else {
                // 非指定路由則不檢查直接下一步
                await Next(context);
            }
        }
    }
}
