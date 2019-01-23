using HtmlAgilityPack;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace XWidget.Web.AddThis {
    public static class AddThisMiddleware {
        private static string AddThisJsTemplate = null;
        private class AddThis {

        }
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
            return
                app.Use(async (context, next) => {
                    var pubid = pubidFunc(context);
                    var addThisStack = context.Features[typeof(AddThis)] as Stack<string>;
                    if (addThisStack == null) {
                        addThisStack = new Stack<string>();
                        context.Features[typeof(AddThis)] = addThisStack;
                    }
                    addThisStack.Push(pubidFunc(context));
                    await next();
                    addThisStack.Pop();
                }).UseHtmlHandler(async (context, html) => {
                    var addThisStack = context.Features[typeof(AddThis)] as Stack<string>;
                    var pubid = addThisStack?.Peek();

                    // 剖析HTML
                    HtmlDocument htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(html);

                    // 取得Body Element並注入腳本
                    var bodyNode = htmlDoc.DocumentNode.SelectSingleNode("//body");
                    if (bodyNode != null) {
                        if (!string.IsNullOrWhiteSpace(pubid)) {
                            bodyNode.InnerHtml += AddThisJsTemplate.Replace("{{pubid}}", pubid);
                        }
                    }

                    return htmlDoc.DocumentNode.OuterHtml;
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
