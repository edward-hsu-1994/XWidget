using HtmlAgilityPack;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace XWidget.Web.GoogleAnalytics {
    public static class GoogleAnalyticsMiddleware {
        private static string GTagJsTemplate = null;
        private class GoogleAnalytics {

        }
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
            return
                app.Use(async (context, next) => {
                    var pubid = trackingCodeFunc(context);
                    var googleAnalyticsStack = context.Features[typeof(GoogleAnalytics)] as Stack<string>;
                    if (googleAnalyticsStack == null) {
                        googleAnalyticsStack = new Stack<string>();
                        context.Features[typeof(GoogleAnalytics)] = googleAnalyticsStack;
                    }
                    googleAnalyticsStack.Push(trackingCodeFunc(context));
                    await next();
                    googleAnalyticsStack.Pop();
                }).UseHtmlHandler(async (context, html) => {
                    var googleAnalyticsStack = context.Features[typeof(GoogleAnalytics)] as Stack<string>;
                    var trackingCode = googleAnalyticsStack?.Peek();

                    // 剖析HTML
                    HtmlDocument htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(html);

                    // 取得Body Element並注入腳本
                    var bodyNode = htmlDoc.DocumentNode.SelectSingleNode("//body");
                    if (bodyNode != null) {
                        if (!string.IsNullOrWhiteSpace(trackingCode)) {
                            bodyNode.InnerHtml += GTagJsTemplate.Replace("{{trackingCode}}", trackingCode);
                        }
                    }

                    return htmlDoc.DocumentNode.OuterHtml;
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
