using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace XWidget.Web.Jwt {
    /// <summary>
    /// JWT標頭
    /// </summary>
    public class DefaultJwtHeader : IJwtHeader {
        /// <summary>
        /// 簽名演算法
        /// </summary>
        public string Algorithm { get; set; } = SecurityAlgorithms.HmacSha256;

        /// <summary>
        /// 類型
        /// </summary>
        public string Type => "JWT";

        public string ContentType { get; set; }

        public string KeyId { get; set; }
    }
}
