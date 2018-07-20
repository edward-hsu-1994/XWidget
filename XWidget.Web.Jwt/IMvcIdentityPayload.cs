using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace XWidget.Web.Jwt {
    public interface IMvcIdentityPayload : ICommonPayload {
        [JsonProperty(ClaimsIdentity.DefaultRoleClaimType,
            NullValueHandling = NullValueHandling.Ignore)]
        string Role { get; }

        [JsonProperty(ClaimsIdentity.DefaultNameClaimType,
            NullValueHandling = NullValueHandling.Ignore)]
        string Name { get; }
    }
}
