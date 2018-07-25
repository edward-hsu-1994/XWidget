using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace XWidget.Web.Jwt {
    /// <summary>
    /// JWT標頭
    /// </summary>
    public class DefaultJwtHeader : IJwtHeader {
        public string Algorithm { get; set; } = SecurityAlgorithms.HmacSha256;

        public string Type => "JWT";
    }
}
