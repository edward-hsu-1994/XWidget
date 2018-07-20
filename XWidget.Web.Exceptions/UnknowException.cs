using System;
using System.Collections.Generic;
using System.Text;

namespace XWidget.Web.Exceptions {
    /// <summary>
    /// 未知錯誤例外
    /// </summary>
    public class UnknowException : ExceptionBase {
        public UnknowException() : base(500, 0, "未知錯誤", "系統發生未知錯誤") { }
    }
}
