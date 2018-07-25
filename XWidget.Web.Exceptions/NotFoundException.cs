using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace XWidget.Web.Exceptions {
    /// <summary>
    /// 找不到目標例外
    /// </summary>
    public class NotFoundException : ExceptionBase {
        public NotFoundException() : base(
            HttpStatusCode.NotFound,
            3,
            "找不到目標",
            "在系統中找不到您所指定的目標資源") { }
    }
}
