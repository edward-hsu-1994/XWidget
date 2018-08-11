using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace XWidget.Web.Jwt {
    /// <summary>
    /// ASP.net Core認證使用的JWT內容
    /// </summary>
    public class MvcIdentityPayload : CommonPayload {
        /// <summary>
        /// Role
        /// </summary>
        [JsonProperty(ClaimsIdentity.DefaultRoleClaimType,
            NullValueHandling = NullValueHandling.Ignore)]
        public string Role { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        [JsonProperty(ClaimsIdentity.DefaultNameClaimType,
            NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
    }
}
