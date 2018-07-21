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
            4,
            "參數錯誤",
            "您輸入的參數有誤") { }
    }
}
