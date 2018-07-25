using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace XWidget.Web.Jwt {
    /// <summary>
    /// JWT標頭基本介面
    /// </summary>
    /// <typeparam name="TExtension"></typeparam>
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
    }
}
