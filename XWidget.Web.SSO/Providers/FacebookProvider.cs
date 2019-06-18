using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;

namespace XWidget.Web.SSO.Providers {
    public class FacebookProvider : SsoProviderBase {
        private HttpClient client = new HttpClient();

        public FacebookProvider(
            IOptions<DefaultSsoProviderConfiguration<FacebookProvider>> config,
            IHttpClientFactory clientFactory) : base(config.Value) {
            this.client = clientFactory.CreateClient();
        }

        public override string Name => "Facebook";

        public override async Task<string> GetLoginUrlAsync(HttpContext context) {
            var url = new UriBuilder();
            url.Host = "www.facebook.com";
            url.Scheme = "https";
            url.Path = "/v3.2/dialog/oauth";
            url.Query = $"?client_id={Configuration.AppId}&redirect_uri={Uri.EscapeDataString(GetCallbackUrl(context))}&response_type=code&state={GenerateStateCode()}";

            if (Configuration.Scopes != null && Configuration.Scopes.Count > 0) {
                url.Query += "&scope=" + string.Join(",", Configuration.Scopes.Select(x => Uri.EscapeDataString(x)));
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
                    var currentUrl = context.Request.GetAbsoluteUri();
                    var ignoreQuery = new string[] {
                        "code",
                        "state"
                    }.Select(x => x.ToUpper());
                    var okQuery = string.Join("&", currentUrl.Query.Split('&').Where(x => {
                        var name = x.Split(new char[] { '=', '?' }, 2, StringSplitOptions.RemoveEmptyEntries)[0].ToUpper();
                        return !ignoreQuery.Contains(name.ToUpper());
                    }));

                    var callbackUrl = currentUrl.ToString().Split(new char[] { '?' }, 2)[0];
                    if (okQuery?.Length > 0) {
                        callbackUrl += '?' + okQuery;
                    }

                    var responseJson = JObject.Parse(await client.GetStringAsync($"https://graph.facebook.com/v3.2/oauth/access_token?client_id={Configuration.AppId}&redirect_uri={Uri.EscapeDataString(callbackUrl)}&client_secret={Configuration.AppKey}&code={code[0]}"));

                    return responseJson["access_token"].Value<string>();
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
