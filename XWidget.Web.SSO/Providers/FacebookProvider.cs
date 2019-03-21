using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;

namespace XWidget.Web.SSO.Providers {
    public class FacebookProvider : SsoProviderBase<DefaultSsoConfiguration> {
        private HttpClient client = new HttpClient();

        public FacebookProvider(DefaultSsoConfiguration config, HttpClient client) : base(config) {
            this.client = client;
        }

        public override string Name => "Facebook";

        public override async Task<string> GetLoginUrlAsync(HttpContext context) {
            var url = new UriBuilder();
            url.Host = "www.facebook.com";
            url.Scheme = "https";
            url.Path = "/v3.2/dialog/oauth";
            url.Query = $"?client_id={Configuration.AppId}&redirect_uri={Uri.EscapeDataString(GetCallbackUrl(context))}&response_type=token&state={GenerateStateCode()}";

            if (Configuration.Scopes != null && Configuration.Scopes.Count > 0) {
                url.Query += "&scopes=" + string.Join(",", Configuration.Scopes);
            }

            return url.ToString();
        }

        public override async Task<bool> VerifyCallbackRequest(HttpContext context) {
            if (context.Request.Query.TryGetValue("state", out StringValues value)) {
                return VerifyStateCode(value[0]);
            } else {
                return false;
            }
        }
        public override async Task<string> GetLoginCallbackTokenAsync(HttpContext context) {
            try {
                return context.Request.Query["access_token"][0].ToString();
            } catch {
                return null;
            }
        }

        public override async Task<bool> VerifyTokenAsync(string token) {
            try {
                var responseJson = JObject.Parse(await client.GetStringAsync($"https://graph.facebook.com/debug_token?input_token={token}&access_token={Configuration.AppKey}"));

                return responseJson["data"]["id"].Value<string>() == Configuration.AppId;
            } catch {
                return false;
            }
        }

    }
}
