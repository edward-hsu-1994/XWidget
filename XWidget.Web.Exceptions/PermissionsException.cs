using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace XWidget.Web.Exceptions {
    /// <summary>
    /// 權限不足例外
    /// </summary>
    public class PermissionsException : ExceptionBase {
        public PermissionsException() : base(
            HttpStatusCode.Forbidden,
            2,
            "權限不足",
            "您無法進行此操作") { }
    }
}
