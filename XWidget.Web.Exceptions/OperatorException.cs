using System;
using System.Collections.Generic;
using System.Text;

namespace XWidget.Web.Exceptions {
    /// <summary>
    /// 操作錯誤例外
    /// </summary>
    public class OperatorException : ExceptionBase {
        public OperatorException() : base(400, 4, "操作錯誤", "您的操作有誤") { }
    }
}
