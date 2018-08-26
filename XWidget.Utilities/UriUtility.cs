using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace XWidget.Utilities {
    /// <summary>
    /// 針對<see cref="Uri"/>常用方法
    /// </summary>
    public static class UriUtility {
        /// <summary>
        /// 渲染URI
        /// </summary>
        /// <param name="template">URI字串樣板</param>
        /// <param name="routeAndQueryArgs">參數</param>
        /// <returns>渲染後的URI</returns>
        public static Uri Render(
            string template,
            Dictionary<string, object> routeAndQueryArgs) {
            var patten1 = @"\{[^\{\}]+\}";
            var patten2 = @"\[[^\[\]]+\]";
            var argBlocks = new Regex($"(({patten1})|({patten2}))").Matches(template);

            string temp = template;

            List<string> usedKeys = new List<string>();

            for (var i = 0; i < argBlocks.Count; i++) {
                var argName_t = argBlocks[i].Value.Slice(1, argBlocks[i].Value.Length - 1)[1].Trim();
                var argName = argName_t.Split('=')[0].Replace("?", "");

                var argValue = routeAndQueryArgs[argName]?.ToString();

                if (argName_t.Split('=').Length > 1) { // default
                    argValue = argValue ?? argName_t.Split(new char[] { '=' }, 2)[1].ToString();
                } else if (argName_t.Last() == '?') { // nullable
                    argValue = "";
                } else if (argValue == null) {
                    throw new ArgumentNullException(argName);
                }

                temp = temp.Replace(argBlocks[i].Value, argValue);
                usedKeys.Add(argName);
            }

            Uri result = new Uri(temp);

            Dictionary<string, List<string>> query = new Dictionary<string, List<string>>();
            if (result.Query != null) {
                var rawQueryData = result.Query.Replace("?", "");
                if (rawQueryData.Length > 0) {
                    var exQuery = result.Query.Replace("?", "").Split('&')
                        .Select(x => x.Split(new char[] { '=' }, 2)
                        .Select(y => Uri.UnescapeDataString(y))
                        .ToArray()).ToArray();

                    foreach (string[] exQueryData in exQuery) {
                        // 已經存在Key
                        if (!query.ContainsKey(exQueryData[0])) {
                            query[exQueryData[0]] = new List<string>();
                        }

                        query[exQueryData[0]].Add(exQueryData[1]);
                    }
                }
            }

            var unusedKeys = routeAndQueryArgs.Keys.Except(usedKeys);
            foreach (string key in unusedKeys) {
                // 已經存在Key
                if (!query.ContainsKey(key)) {
                    query[key] = new List<string>();
                }

                if (routeAndQueryArgs[key] is IEnumerable newEnum &&
                            !(routeAndQueryArgs[key] is string)) {
                    foreach (var newEnumItem in newEnum) {
                        query[key].Add(newEnumItem.ToString());
                    }

                } else {
                    query[key].Add(routeAndQueryArgs[key].ToString());
                }
            }

            List<string> queryString = new List<string>();
            foreach (string key in query.Keys) {
                if (query[key] is IEnumerable enumData &&
                    !(query[key] is string)) {
                    foreach (var queryItem in enumData) {
                        queryString.Add($"{key}={Uri.EscapeDataString(queryItem.ToString())}");
                    }
                } else {
                    queryString.Add($"{key}={Uri.EscapeDataString(query[key].ToString())}");
                }
            }

            var tempId = Guid.NewGuid().ToString();
            temp = temp.Replace("://", tempId);
            while (temp.IndexOf("//") > -1) { //除去空路由
                temp = temp.Replace("//", "/");
            }
            temp = temp.Replace(tempId, "://");

            if (queryString.Count > 0) {
                return new Uri(temp.Split('?')[0] + "?" + string.Join("&", queryString));
            }

            return new Uri(temp);
        }
    }
}
