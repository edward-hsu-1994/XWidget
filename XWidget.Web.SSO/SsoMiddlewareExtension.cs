using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Runtime.Serialization;

namespace XWidget.Web.SSO {
    public static class SsoMiddlewareExtension {
        /// <summary>
        /// 加入SSO提供者
        /// </summary>
        /// <param name="services">服務集合</param>
        /// <param name="providerTypes">SSO提供者類型</param>
        /// <returns>服務集合</returns>
        public static IServiceCollection AddSsoProviders(this IServiceCollection services, params Type[] providerTypes) {
            if (!providerTypes.All(x => x.GetInterfaces().Any(y => y == typeof(ISsoProvider)))) {
                throw new ArgumentException("providerType require implement ISsoProvider");
            }
            foreach (var type in providerTypes) {
                services.AddSingleton(type);
            }
            services.AddSingleton<ISsoProvider[]>(s => {
                List<ISsoProvider> result = new List<ISsoProvider>();
                foreach (var type in providerTypes) {
                    result.Add((ISsoProvider)s.GetService(type));
                }
                return result.ToArray();
            });

            return services;
        }

        /// <summary>
        /// 加入SSO處理器
        /// </summary>
        /// <typeparam name="THandler">SSO處理器類型</typeparam>
        /// <param name="services">服務集合</param>
        /// <returns>服務集合</returns>
        public static IServiceCollection AddSsoHandler<THandler>(this IServiceCollection services)
            where THandler : class, ISsoHandler {
            return services.AddScoped<ISsoHandler, THandler>();
        }

        /// <summary>
        /// 使用SSO
        /// </summary>
        /// <param name="app">應用程式建構器</param>
        /// <param name="pathMatch">SSO路徑</param>
        /// <param name="onLogin">登入回呼</param>
        /// <param name="onError">錯誤回呼</param>
        /// <returns>應用程式建構器</returns>
        public static IApplicationBuilder UseSso(
            this IApplicationBuilder app,
            PathString pathMatch,
            Action<ISsoProvider, string, HttpContext> onLogin,
            Action<ISsoProvider, HttpContext> onError
            ) {
            return app.Use(async (context, next) => {
                if (!context.Request.Path.StartsWithSegments(pathMatch)) {
                    await next();
                    return;
                }

                var providers = context.RequestServices.GetService<ISsoProvider[]>();

                foreach (var provider in providers) {
                    if (context.Request.Path.StartsWithSegments(pathMatch + "/" + provider.Name + "/login")) {
                        context.Response.Redirect(await provider.GetLoginUrlAsync(context));
                        return;
                    }
                    if (context.Request.Path.StartsWithSegments(pathMatch + "/" + provider.Name + "/login-callback")) {
                        if (!await provider.VerifyCallbackRequest(context)) {
                            context.Response.StatusCode = 400;
                            onError(provider, context);
                            return;
                        }

                        var token = await provider.GetLoginCallbackTokenAsync(context);

                        if (token == null) {
                            context.Response.StatusCode = 400;
                            onError(provider, context);
                            return;
                        }

                        if (await provider.VerifyTokenAsync(token)) {
                            onLogin(provider, token, context);
                            return;
                        } else {
                            context.Response.StatusCode = 400;
                            onError(provider, context);
                            return;
                        }
                    }
                }

                await next();
            });
        }

        /// <summary>
        /// 使用SSO
        /// </summary>
        /// <typeparam name="THandler">SSO處理介面</typeparam>
        /// <param name="app">應用程式建構器</param>
        /// <param name="pathMatch">SSO路徑</param>
        /// <returns>應用程式建構器</returns>
        public static IApplicationBuilder UseSso<THandler>(
            this IApplicationBuilder app,
            PathString pathMatch)
            where THandler : ISsoHandler, new() {
            return app.Use(async (context, next) => {
                if (!context.Request.Path.StartsWithSegments(pathMatch)) {
                    await next();
                    return;
                }

                var providers = context.RequestServices.GetService<ISsoProvider[]>();

                foreach (var provider in providers) {
                    if (context.Request.Path.StartsWithSegments(pathMatch + "/" + provider.Name + "/login")) {
                        context.Response.Redirect(await provider.GetLoginUrlAsync(context));
                        return;
                    }
                    if (context.Request.Path.StartsWithSegments(pathMatch + "/" + provider.Name + "/login-callback")) {
                        var handler = context.RequestServices.GetService<ISsoHandler>();
                        if (!await provider.VerifyCallbackRequest(context)) {
                            context.Response.StatusCode = 400;
                            await handler.OnError(provider, context);
                            return;
                        }

                        var token = await provider.GetLoginCallbackTokenAsync(context);

                        if (token == null) {
                            context.Response.StatusCode = 400;
                            await handler.OnError(provider, context);
                            return;
                        }

                        if (await provider.VerifyTokenAsync(token)) {
                            await handler.OnLogin(provider, token, context);
                            return;
                        } else {
                            context.Response.StatusCode = 400;
                            await handler.OnError(provider, context);
                            return;
                        }
                    }
                }

                await next();
            });
        }
    }
}
