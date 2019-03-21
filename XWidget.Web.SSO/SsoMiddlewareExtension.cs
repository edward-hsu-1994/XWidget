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
            ) where TSSOHandler : ISSOHandler {
            return app.Map(pathMatch, app2 => {
                foreach (var provider in Providers.GetAllProviders()) {
                    app2.Map($"/{provider}/bind", app3 => {
                        app3.Run(async context => {
                        });
                    });
                    app2.Map($"/{provider}/login", app3 => {
                        app3.Run(async context => {
                            Providers.GetProviderInstance(provider);

                        });
                    });
                    app2.Map($"/{provider}/bind-callback", app3 => {
                        app3.Run(async context => {


                            Providers.GetProviderInstance(provider);

                        });
                    });
                    app2.Map($"/{provider}/login-callback", app3 => {
                        app3.Run(async context => {
                            Providers.GetProviderInstance(provider);

                        });
                    });
                }
            });
        }
    }
}
