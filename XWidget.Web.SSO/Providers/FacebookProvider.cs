using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

namespace XWidget.Web.SSO.Providers {
    public class FacebookProvider : SsoProviderBase<DefaultSsoConfiguration> {
        private HttpClient client = new HttpClient();

        public override string Name => "Facebook";

        public override async Task<string> GetLoginCallbackToken(HttpContext context) {
            return context.Request.Query["access_token"][0].ToString();
        }

        public override async Task<string> GetLoginUrl(HttpContext context) {
            var url = new UriBuilder();
            url.Host = "www.facebook.com";
            url.Scheme = "https";
            url.Path = "/v3.2/dialog/oauth";
            url.Query = $"?client_id={Configuration.AppId}&redirect_uri={GetCallbackUrl(context)}&response_type=token&state={GenerateStateCode()}";

            if (Configuration.Scopes != null && Configuration.Scopes.Count > 0) {
                url.Query += "&scopes=" + string.Join(",", Configuration.Scopes);
            }

            return url.ToString();
        }

        public override async Task<bool> VerifyToken(string token) {
            try {
                var responseJson = JObject.Parse(await client.GetStringAsync($"https://graph.facebook.com/debug_token?input_token={token}&access_token={Configuration.AppKey}"));

                return responseJson["data"]["id"].Value<string>() == Configuration.AppId;
            } catch {
                return false;
            }
        }
    }
}
