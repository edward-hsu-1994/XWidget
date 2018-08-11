using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace XWidget.Web.Exceptions {
    /// <summary>
    /// 未知錯誤例外
    /// </summary>
    public class UnknowException : ExceptionBase {
        public UnknowException() : base(
            HttpStatusCode.InternalServerError,
            0,
            "未知錯誤",
            "系統發生未知錯誤") { }

        public UnknowException(string message) : base(
            HttpStatusCode.InternalServerError,
            0,
            "未知錯誤",
            message) { }

        public UnknowException(string name, string message) : base(
            HttpStatusCode.InternalServerError,
            1,
            name,
            message) { }

        public UnknowException(int code, string name, string message) : base(
            HttpStatusCode.InternalServerError,
            code,
            name,
            message) { }
    }
}
