using HtmlAgilityPack;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace XWidget.Web.AddThis {
    public static class AddThisMiddleware {
        private static string AddThisJsTemplate = null;
        static AddThisMiddleware() {
            var assembly = Assembly.GetExecutingAssembly();
            var textStreamReader = new StreamReader(assembly.GetManifestResourceStream("XWidget.Web.AddThis.addThis.html"));
            AddThisJsTemplate = textStreamReader.ReadToEnd();
        }

        /// <summary>
        /// 在text/html類型的結果中注入AddThis社群分享按鈕腳本
        /// </summary>
        /// <param name="app">應用程式建構器</param>
        /// <param name="pubidFunc">網頁代碼產生方法</param>
        /// <returns>應用程式建構器</returns>
        public static IApplicationBuilder UseAddThisShareButton(
            this IApplicationBuilder app,
            Func<HttpContext, string> pubidFunc
            ) {
            return app.Use(async (context, next) => {
                var pubid = pubidFunc(context);
                var originStream = context.Response.Body;

                var fakeBody = new MemoryStream();

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
                        if (!string.IsNullOrWhiteSpace(pubid)) {
                            bodyNode.InnerHtml += AddThisJsTemplate.Replace("{{pubid}}", pubid);
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
                context.Response.ContentLength = context.Response.ContentLength + fakeBody.Length;

                await fakeBody.CopyToAsync(originStream);
            });
        }

        /// <summary>
        /// 在text/html類型的結果中注入AddThis社群分享按鈕腳本
        /// </summary>
        /// <param name="app">應用程式建構器</param>
        /// <param name="pubidFunc">網頁代碼產生方法</param>
        /// <returns>應用程式建構器</returns>
        public static IApplicationBuilder UseAddThisShareButton(
            this IApplicationBuilder app,
            Func<HttpContext, Task<string>> pubidFunc) {
            return app.UseAddThisShareButton(c => pubidFunc(c).GetAwaiter().GetResult());
        }

        /// <summary>
        /// 在text/html類型的結果中注入AddThis社群分享按鈕腳本
        /// </summary>
        /// <param name="app">應用程式建構器</param>
        /// <param name="pubid">網頁代碼</param>
        /// <returns>應用程式建構器</returns>
        public static IApplicationBuilder UseAddThisShareButton(
            this IApplicationBuilder app,
            string pubid) {
            return app.UseAddThisShareButton(x => pubid);
        }
    }
}
