using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace XWidget.Web {
    public static class HttpRequestExtension {
        /// <summary>
        /// 取得網址
        /// </summary>
        /// <param name="request"></param>
        /// <returns>網址</returns>
        public static Uri GetAbsoluteUri(this HttpRequest request) {
            return new Uri(
                string.Concat(
                       request.Scheme,
                       "://",
                       request.Host.ToUriComponent(),
                       request.PathBase.ToUriComponent(),
                       request.Path.ToUriComponent(),
                       request.QueryString.ToUriComponent())
                );
        }
    }
}
