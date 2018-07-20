using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace XWidget.Web.Jwt {
    public interface ICommonPayload {
        /// <summary>
        /// 發行者
        /// </summary>
        string Issuer { get; }

        /// <summary>
        /// 接受者
        /// </summary>
        string Audience { get; }

        /// <summary>
        /// 行為者
        /// </summary>
        string Actor { get; }

        /// <summary>
        /// 主題
        /// </summary>
        string Subject { get; }

        /// <summary>
        /// 有效期限起始
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(JwtTimeConvert))]
        DateTime? IssuedAt { get; }

        /// <summary>
        /// 有效期限結束
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(JwtTimeConvert))]
        DateTime? Expires { get; }
    }
}
