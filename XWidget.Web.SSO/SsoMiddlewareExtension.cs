using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace XWidget.Web.SSO {
    public static class SsoMiddlewareExtension {

        public static IApplicationBuilder UseSSO<TSSOHandler>(
            this IApplicationBuilder app,
            PathString pathMatch
            ) where TSSOHandler : ISsoHandler {
            return app.Map(pathMatch, (Action<IApplicationBuilder>)(app2 => {
                foreach (var provider in Providers.GetAllProviders()) {
                    app2.Map($"/{provider}/login", (Action<IApplicationBuilder>)(app3 => {
                        app3.Run((RequestDelegate)(async context => {
                            Providers.GetProviderInstance(provider);

                        }));
                    }));
                    app2.Map($"/{provider}/login-callback", (Action<IApplicationBuilder>)(app3 => {
                        app3.Run((RequestDelegate)(async context => {
                            Providers.GetProviderInstance(provider);

                        }));
                    }));
                }
            }));
        }
    }
}
