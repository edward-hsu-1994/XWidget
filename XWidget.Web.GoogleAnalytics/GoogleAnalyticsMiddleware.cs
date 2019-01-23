using HtmlAgilityPack;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace XWidget.Web.GoogleAnalytics {
    public static class GoogleAnalyticsMiddleware {
        private static string GTagJsTemplate = null;
        static GoogleAnalyticsMiddleware() {
            var assembly = Assembly.GetExecutingAssembly();
            var textStreamReader = new StreamReader(assembly.GetManifestResourceStream("XWidget.Web.GoogleAnalytics.gtag.html"));
            GTagJsTemplate = textStreamReader.ReadToEnd();
        }

        /// <summary>
        /// 在text/html類型的結果中注入Google Analytics腳本
        /// </summary>
        /// <param name="app">應用程式建構器</param>
        /// <param name="trackingCodeFunc">追蹤碼產生方法</param>
        /// <returns>應用程式建構器</returns>
        public static IApplicationBuilder UseGoogleAnalytics(
            this IApplicationBuilder app,
            Func<HttpContext, string> trackingCodeFunc
            ) {
            return app.Use(async (context, next) => {
                var trackingCode = trackingCodeFunc(context);
                var originStream = context.Response.Body;
                long? originStreamLength = context.Response.ContentLength;

                Stream fakeBody = new MemoryStream();

                context.Response.Body = fakeBody;
                await next();

                if (context.Response.ContentType == "text/html") {
                    fakeBody.Seek(0, SeekOrigin.Begin);
                    // 讀取HTML內容
                    var rawHtml = await new StreamReader(fakeBody).ReadToEndAsync();
                    // 剖析HTML
                    HtmlDocument html = new HtmlDocument();
                    html.LoadHtml(rawHtml);

                    // 取得Body Element並注入腳本
                    var bodyNode = html.DocumentNode.SelectSingleNode("//body");
                    if (bodyNode != null) {
                        if (!string.IsNullOrWhiteSpace(trackingCode)) {
                            bodyNode.InnerHtml += GTagJsTemplate.Replace("{{trackingCode}}", trackingCode);
                        }
                    }

                    // 字串轉Stream
                    fakeBody = new MemoryStream();
                    StreamWriter streamWriter = new StreamWriter(fakeBody);
                    streamWriter.Write(html.DocumentNode.OuterHtml);
                    streamWriter.Flush();
                }
                fakeBody.Seek(0, SeekOrigin.Begin);

                context.Response.Body = originStream;
                context.Response.ContentLength = (originStreamLength ?? 0) + fakeBody.Length;

                await fakeBody.CopyToAsync(originStream);
            });
        }

        /// <summary>
        /// 在text/html類型的結果中注入Google Analytics腳本
        /// </summary>
        /// <param name="app">應用程式建構器</param>
        /// <param name="trackingCodeFunc">追蹤碼產生方法</param>
        /// <returns>應用程式建構器</returns>
        public static IApplicationBuilder UseGoogleAnalytics(
            this IApplicationBuilder app,
            Func<HttpContext, Task<string>> trackingCodeFunc) {
            return app.UseGoogleAnalytics(c => trackingCodeFunc(c).GetAwaiter().GetResult());
        }

        /// <summary>
        /// 在text/html類型的結果中注入Google Analytics腳本
        /// </summary>
        /// <param name="app">應用程式建構器</param>
        /// <param name="trackingCode">追蹤碼</param>
        /// <returns>應用程式建構器</returns>
        public static IApplicationBuilder UseGoogleAnalytics(
            this IApplicationBuilder app,
            string trackingCode
            ) {
            return app.UseGoogleAnalytics(c => trackingCode);
        }
    }
}
