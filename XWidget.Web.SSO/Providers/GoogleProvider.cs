using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;

namespace XWidget.Web.SSO.Providers {
    public class GoogleProvider : SsoProviderBase {
        private HttpClient client = new HttpClient();

        public GoogleProvider(
            IOptions<DefaultSsoProviderConfiguration<GoogleProvider>> config,
            IHttpClientFactory clientFactory) : base(config.Value) {
            this.client = clientFactory.CreateClient();
        }

        public override string Name => "Google";

        public override async Task<string> GetLoginUrlAsync(HttpContext context) {
            var url = new UriBuilder();
            url.Host = "accounts.google.com";
            url.Scheme = "https";
            url.Path = "/o/oauth2/v2/auth";
            url.Query = $"?client_id={Configuration.AppId}&access_type=offline&redirect_uri={Uri.EscapeDataString(GetCallbackUrl(context))}&response_type=code&state={GenerateStateCode()}";

            if (Configuration.Scopes != null && Configuration.Scopes.Count > 0) {
                url.Query += "&scope=" + string.Join("%20", Configuration.Scopes.Select(x => Uri.EscapeDataString(x)));
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

                    //var ignoreQuery = new string[] {
                    //    "code",
                    //    "state",
                    //    "scope"
                    //}.Select(x => x.ToUpper());

                    //var okQuery = string.Join("&", currentUrl.Query.Split('&').Where(x => {
                    //    var name = x.Split(new char[] { '=', '?' }, 2, StringSplitOptions.RemoveEmptyEntries)[0].ToUpper();
                    //    return !ignoreQuery.Contains(name.ToUpper());
                    //}));

                    var callbackUrl = currentUrl.ToString().Split(new char[] { '?' }, 2)[0];

                    //if (okQuery?.Length > 0) {
                    //    callbackUrl += '?' + okQuery;
                    //}

                    Dictionary<string, string> formDataDictionary = new Dictionary<string, string>() {
                        ["client_id"] = Configuration.AppId,
                        ["client_secret"] = Configuration.AppKey,
                        ["redirect_uri"] = callbackUrl,
                        ["grant_type"] = "authorization_code",
                        ["code"] = code[0]
                    };

                    var formData = new FormUrlEncodedContent(formDataDictionary);

                    var response = await client.PostAsync("https://www.googleapis.com/oauth2/v4/token", formData);

                    var responseJson = JObject.Parse(await response.Content.ReadAsStringAsync());

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
