using HtmlAgilityPack;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Reflection;

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
        /// <param name="trackingCode">追蹤碼</param>
        /// <returns>應用程式建構器</returns>
        public static IApplicationBuilder UseGoogleAnalytics(
            this IApplicationBuilder app,
            string trackingCode
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
                        baseNode.InnerHtml += GTagJsTemplate.Replace("{{trackingCode}}", trackingCode);
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
    }
}
