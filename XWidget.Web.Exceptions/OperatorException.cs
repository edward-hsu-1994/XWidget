using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace XWidget.Web.Exceptions {
    /// <summary>
    /// 操作錯誤例外
    /// </summary>
    public class OperatorException : ExceptionBase {
        public OperatorException() : base(
            HttpStatusCode.BadRequest,
            4,
            "操作錯誤",
            "您的操作有誤") { }

        public OperatorException(string message) : base(
            HttpStatusCode.BadRequest,
            4,
            "操作錯誤",
            message) { }

        public OperatorException(string name, string message) : base(
            HttpStatusCode.BadRequest,
            1,
            name,
            message) { }

        public OperatorException(int code, string name, string message) : base(
            HttpStatusCode.BadRequest,
            code,
            name,
            message) { }
    }
}
