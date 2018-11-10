using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.AspNetCore.Mvc {
    /// <summary>
    /// 控制器擴充
    /// </summary>
    public static class ControllerExtension {
        /// <summary>
        /// 重定向並在client端進行Post FormData行為
        /// </summary>
        /// <param name="obj">控制器實例</param>
        /// <param name="url">目標網址</param>
        /// <param name="formData">FormData</param>
        /// <returns>重定向HTML</returns>
        public static IActionResult TestRedirectWithClientPostFormData(this Controller obj, string url, Dictionary<string, string> formData) {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(@"<html><head></head><body><form id=""frm1"" name=""frm1""></form><script>document.frm1.submit()</script></body></html>");

            HtmlNode formNode = doc.DocumentNode.SelectSingleNode("//form");

            if (formNode != null) {
                formNode.SetAttributeValue("method", "POST");
                formNode.SetAttributeValue("action", url);
                formData.Keys.ForEach(key => formNode.AppendChild(HtmlNode.CreateNode($"<input type=\"hidden\" name=\"{key}\" value=\"{formData[key]}\" />")));
            }

            return obj.Content(doc.DocumentNode.OuterHtml, "text/html");
        }
    }
}
