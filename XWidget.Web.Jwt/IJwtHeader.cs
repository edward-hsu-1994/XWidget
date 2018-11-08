using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace XWidget.Web.Jwt {
    /// <summary>
    /// JWT標頭基本介面，請至 https://tools.ietf.org/html/rfc7515 查看
    /// </summary>
    public interface IJwtHeader {
        /// <summary>
        /// 簽名演算法
        /// </summary>
        [JsonProperty("alg")]
        string Algorithm { get; set; }

        /// <summary>
        /// 類型
        /// </summary>
        [JsonProperty("typ")]
        string Type { get; }

        /// <summary>
        /// 給予JWS使用的ContentType, 請至 http://www.iana.org/assignments/media-types/media-types.xhtml 查看
        /// </summary>
        [JsonProperty("cty", NullValueHandling = NullValueHandling.Ignore)]
        string ContentType { get; set; }

        /// <summary>
        /// 指定JWS將使用哪個密鑰，請至 https://tools.ietf.org/html/rfc7515#page-11 查看
        /// </summary>
        [JsonProperty("kid", NullValueHandling = NullValueHandling.Ignore)]
        string KeyId { get; set; }
    }
}
