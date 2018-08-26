using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace XWidget.Rest {
    /// <summary>
    /// 網址渲染器
    /// </summary>
    internal class UrlRender {
        public string DefaultRoute { get; set; }

        public UrlRender(string defaultRoute) {
            DefaultRoute = defaultRoute;
        }

        private string urlCombine(params string[] urls) {
            string result = string.Join("/", urls);
            while (result.IndexOf("//") > -1) {
                result = result.Replace("//", "/");
            }
            return result;
        }

        /// <summary>
        /// 渲染網址
        /// </summary>
        /// <param name="path">位址</param>
        /// <param name="args">參數</param>
        /// <returns>渲染結果</returns>
        public string Render(string path, Dictionary<string, object> args) {
            var result = urlCombine(DefaultRoute, path);

            // https://example.com/api/[controller]/{myaction}

            return result;
        }

    }
}
