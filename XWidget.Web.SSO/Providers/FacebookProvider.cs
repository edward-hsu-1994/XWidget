using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace XWidget.Web.SSO.Providers {
    public class FacebookProvider : SsoProviderBase<DefaultSsoConfiguration> {
        public override string Name => "Facebook";

        public override string GetLoginCallbackToken(HttpContext context) {
            return context.Request.Query["access_token"][0].ToString();
        }

        public override string GetLoginUrl(HttpContext context) {
            var url = new UriBuilder();
            url.Host = "www.facebook.com";
            url.Scheme = "https";
            url.Path = "/v3.2/dialog/oauth";
            url.Query = $"?client_id={Configuration.AppId}&redirect_uri={GetCallbackUrl(context)}&response_type=token&state={GenerateStateCode()}";

            return url.ToString();
        }

        public override bool VerifyToken(string token) {
            throw new NotImplementedException();
        }
    }
}
