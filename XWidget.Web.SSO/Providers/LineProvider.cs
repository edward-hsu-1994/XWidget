using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;

namespace XWidget.Web.SSO.Providers {
    public class LineProvider : SsoProviderBase {
        private HttpClient client = new HttpClient();

        public LineProvider(DefaultProviderConfiguration<LineProvider> config, IHttpClientFactory clientFactory) : base(config) {
            this.client = clientFactory.CreateClient();
        }

        public override string Name => "Line";

        public override async Task<string> GetLoginUrlAsync(HttpContext context) {
            var url = new UriBuilder();
            url.Host = "access.line.me";
            url.Scheme = "https";
            url.Path = "/oauth2/v2.1/authorize";
            url.Query = $"?clientId={Configuration.AppId}&response_type=code&redirect_uri={Uri.EscapeDataString(GetCallbackUrl(context))}&state={GenerateStateCode()}";

            if (Configuration.Scopes != null && Configuration.Scopes.Count > 0) {
                url.Query += "&scope=" + string.Join("%20", Configuration.Scopes);
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
            if (context.Request.Query.TryGetValue("code", out StringValues code)) {
                try {
                    var currentUrl = context.Request.GetAbsoluteUri().ToString();
                    currentUrl = currentUrl.Split(new char[] { '?' }, 2)[0];

                    var dict = new Dictionary<string, string>();
                    dict.Add("grant_type", "authorization_code");
                    dict.Add("code", code);
                    dict.Add("redirect_uri", currentUrl);
                    dict.Add("client_id", Configuration.AppId);
                    dict.Add("client_secret", Configuration.AppKey);

                    var req = new HttpRequestMessage(HttpMethod.Post, "https://api.line.me/oauth2/v2.1/token") { Content = new FormUrlEncodedContent(dict) };
                    var res = await client.SendAsync(req);

                    if (!res.IsSuccessStatusCode) {
                        return null;
                    }

                    var responseJson = JObject.Parse(await res.Content.ReadAsStringAsync());

                    return responseJson["token_type"].Value<string>() + " " + responseJson["access_token"].Value<string>();
                } catch {
                    return null;
                }
            } else {
                return null;
            }
        }

        public override async Task<bool> VerifyTokenAsync(string token) {
            return token != null;
        }

    }
}
