using HtmlAgilityPack;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
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
                var originStream = context.Response.Body;
                var warpStream = new MemoryStream();

                context.Response.Body = warpStream;
                await next();

                if (context.Response.ContentType == "text/html") {
                    warpStream.Seek(0, SeekOrigin.Begin);
                    // 讀取HTML內容
                    var rawHtml = await new StreamReader(warpStream).ReadToEndAsync();
                    // 剖析HTML
                    HtmlDocument html = new HtmlDocument();
                    html.LoadHtml(rawHtml);

                    // 取得BaseElement並設定href
                    var baseNode = html.DocumentNode.SelectSingleNode("//body");
                    if (baseNode != null) {
                        baseNode.InnerHtml += AddThisJsTemplate.Replace("{{pubid}}", pubidFunc(context));
                    }

                    warpStream = new MemoryStream();
                    StreamWriter streamWriter = new StreamWriter(warpStream);
                    streamWriter.Write(html.DocumentNode.OuterHtml);
                    streamWriter.Flush();
                }
                warpStream.Seek(0, SeekOrigin.Begin);

                context.Response.Body = originStream;

                #region Backup Response Properties
                var backup = new {
                    context.Response.ContentType,
                    context.Response.StatusCode
                };
                #endregion

                context.Response.Clear();

                #region Reset Response Properties
                context.Response.ContentType = backup.ContentType;
                context.Response.StatusCode = backup.StatusCode;
                #endregion

                await warpStream.CopyToAsync(context.Response.Body);
                context.Response.ContentLength = warpStream.Length;
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
