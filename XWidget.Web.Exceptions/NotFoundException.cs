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

        public NotFoundException(string message) : base(
            HttpStatusCode.NotFound,
            3,
            "找不到目標",
            message) { }

        public NotFoundException(string name, string message) : base(
            HttpStatusCode.NotFound,
            1,
            name,
            message) { }

        public NotFoundException(int code, string name, string message) : base(
            HttpStatusCode.NotFound,
            code,
            name,
            message) { }
    }
}
