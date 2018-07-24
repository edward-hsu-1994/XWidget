using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace XWidget.Web.Jwt {
    public class CommonPayload {
        /// <summary>
        /// 發行者
        /// </summary>
        [JsonProperty("iss")]
        public string Issuer { get; set; }

        /// <summary>
        /// 接受者
        /// </summary>
        [JsonProperty("aud")]
        public string Audience { get; set; }

        /// <summary>
        /// 行為者
        /// </summary>
        [JsonProperty("act")]
        public string Actor { get; set; }

        /// <summary>
        /// 主題
        /// </summary>
        [JsonProperty("sub")]
        public string Subject { get; set; }

        /// <summary>
        /// 有效期限起始
        /// </summary>
        [JsonProperty("iat", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(JwtTimeConvert))]
        public DateTime? IssuedAt { get; set; }

        /// <summary>
        /// 有效期限結束
        /// </summary>
        [JsonProperty("exp", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(JwtTimeConvert))]
        public DateTime? Expires { get; set; }
    }
}
