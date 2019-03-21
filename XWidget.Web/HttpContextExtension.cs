using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace XWidget.Web {
    public static class HttpContextExtension {
        /// <summary>
        /// 取得網址
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns>網址</returns>
        public static Uri GetAbsoluteUri(this HttpContext httpContext) {
            return new Uri(
                string.Concat(
                       httpContext.Request.Scheme,
                       "://",
                       httpContext.Request.Host.ToUriComponent(),
                       httpContext.Request.PathBase.ToUriComponent(),
                       httpContext.Request.Path.ToUriComponent(),
                       httpContext.Request.QueryString.ToUriComponent())
                );
        }
    }
}
