using System;
using System.Collections.Generic;
using System.Text;

namespace XWidget.Web.Jwt {
    /// <summary>
    /// JWT基本介面
    /// </summary>
    /// <typeparam name="TPayload">主內容類型</typeparam>
    public interface IJwtToken<THeader, TPayload>
        where THeader : IJwtHeader {
        /// <summary>
        /// 標頭
        /// </summary>
        THeader Header { get; set; }

        /// <summary>
        /// 內容
        /// </summary>
        TPayload Payload { get; set; }
    }
}
