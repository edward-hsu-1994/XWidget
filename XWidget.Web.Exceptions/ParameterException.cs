using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace XWidget.Web.Exceptions {
    /// <summary>
    /// 參數錯誤例外
    /// </summary>
    public class ParameterException : ExceptionBase {
        public ParameterException() : base(
            HttpStatusCode.BadRequest,
            5,
            "參數錯誤",
            "您輸入的參數有誤") { }

        public ParameterException(string message) : base(
            HttpStatusCode.BadRequest,
            5,
            "參數錯誤",
            message) { }

        public ParameterException(string name, string message) : base(
            HttpStatusCode.BadRequest,
            1,
            name,
            message) { }

        public ParameterException(int code, string name, string message) : base(
            HttpStatusCode.BadRequest,
            code,
            name,
            message) { }
    }
}
