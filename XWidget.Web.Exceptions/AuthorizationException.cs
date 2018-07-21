using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace XWidget.Web.Exceptions {
    /// <summary>
    /// 授權無效例外
    /// </summary>
    public class AuthorizationException : ExceptionBase {
        public AuthorizationException() : base(
            HttpStatusCode.Unauthorized,
            1,
            "授權無效",
            "您目前尚未登入或您目前使用的授權失效，您可以嘗試重新登入或重新取得授權") { }
    }
}
