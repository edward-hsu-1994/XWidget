using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;

namespace XWidget.Web.SSO.Providers {
    public class GithubProvider : SsoProviderBase {
        private HttpClient client = new HttpClient();

        public GithubProvider(DefaultProviderConfiguration<GithubProvider> config, IHttpClientFactory clientFactory) : base(config) {
            this.client = clientFactory.CreateClient();
        }

        public override string Name => "Github";

        public override async Task<string> GetLoginUrlAsync(HttpContext context) {
            var url = new UriBuilder();
            url.Host = "github.com";
            url.Scheme = "https";
            url.Path = "/login/oauth/authorize";
            url.Query = $"?client_id={Configuration.AppId}&redirect_uri={Uri.EscapeDataString(GetCallbackUrl(context))}&state={GenerateStateCode()}";

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
                    var request = new System.Net.Http.HttpRequestMessage(new HttpMethod(HttpMethods.Post), $"https://github.com/login/oauth/access_token?client_id={Configuration.AppId}&client_secret={Configuration.AppKey}&code={code[0]}");
                    request.Headers.TryAddWithoutValidation("accept", "application/json");

                    var response = await client.SendAsync(request);

                    var responseString = await response.Content.ReadAsStringAsync();

                    var responseJson = JObject.Parse(responseString);

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
