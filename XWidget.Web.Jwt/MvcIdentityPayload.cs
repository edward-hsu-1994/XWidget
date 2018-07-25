using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace XWidget.Web.Jwt {
    public class MvcIdentityPayload : CommonPayload {
        [JsonProperty(ClaimsIdentity.DefaultRoleClaimType,
            NullValueHandling = NullValueHandling.Ignore)]
        public string Role { get; set; }

        [JsonProperty(ClaimsIdentity.DefaultNameClaimType,
            NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
    }
}
