using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Newtonsoft.Json;
using XWidget.Reflection;
using XWidget.Utilities;

namespace XWidget.Rest {
    internal class RestInterceptor : IInterceptor {
        private Uri BuildUri(RouteAttribute route, HttpMethodAttribute method, Dictionary<string, object> routeOrQueryArgs) {
            string url = "";
            if (route != null && route.Template != null) {
                url += route.Template;
            }
            if (method != null && method.Template != null) {
                url += method.Template;
            }

            return UriUtility.Render(url, routeOrQueryArgs);
        }

        public async Task InterceptAsync(IInvocation invocation) {
            var client = new HttpClient();

            var route = invocation.TargetType.GetCustomAttribute<RouteAttribute>();

            var httpGet = invocation.Method.GetCustomAttribute<HttpGetAttribute>();
            var httpPost = invocation.Method.GetCustomAttribute<HttpPostAttribute>();
            var httpPut = invocation.Method.GetCustomAttribute<HttpPutAttribute>();
            var httpDelete = invocation.Method.GetCustomAttribute<HttpDeleteAttribute>();

            HttpMethod method = HttpMethod.Get;
            if (httpGet != null) {
                method = HttpMethod.Get;
            } else if (httpPost != null) {
                method = HttpMethod.Post;
            } else if (httpPut != null) {
                method = HttpMethod.Put;
            } else if (httpDelete != null) {
                method = HttpMethod.Delete;
            } else {
                if (invocation.TargetType.IsClass &&
                    !invocation.Method.IsAbstract) {
                    invocation.Proceed();
                    return;
                }
                throw new NotSupportedException();
            }

            var parameters = invocation.Method.GetParameters();

            Dictionary<string, object> routeOrQueryArgs = new Dictionary<string, object>();
            bool isForm = false, isBody = false;

            var formContent = new MultipartFormDataContent();
            HttpContent bodyContent = null;

            #region 參數設定
            foreach (var param in parameters) {
                var query_arg = param.GetCustomAttribute<FromQueryAttribute>();
                var route_arg = param.GetCustomAttribute<FromRouteAttribute>();
                var form_arg = param.GetCustomAttribute<FromFormAttribute>();
                var body_arg = param.GetCustomAttribute<FromBodyAttribute>();
                var header_arg = param.GetCustomAttribute<FromHeaderAttribute>();

                var paramName = query_arg?.Name ?? route_arg?.Name ?? form_arg?.Name ?? header_arg?.Name ?? param.Name;

                var paramValue = invocation.GetArgumentValue(param.Position);

                if (paramValue == null) {
                    if (param.HasDefaultValue) {
                        paramValue = param.DefaultValue;
                    }

                    if (paramValue == null) {
                        continue;
                    }
                }

                if (query_arg != null || route_arg != null) {
                    routeOrQueryArgs[paramName] = paramValue;
                } else if (form_arg != null && !isBody) {
                    if (paramValue is FileInfo fileInfo) {
                        formContent.Add(new StreamContent(fileInfo.Open(FileMode.Open)), paramName);
                    } else if (paramValue is IEnumerable<FileInfo> fileInfos) {
                        foreach (var item in fileInfos) {
                            formContent.Add(new StreamContent(item.Open(FileMode.Open)), paramName);
                        }
                    } else {
                        throw new NotSupportedException();
                    }
                    isForm = true;
                } else if (body_arg != null && !isForm) {
                    if (paramValue is Stream) {
                        throw new NotSupportedException();
                    } else {
                        bodyContent = new StringContent(JsonConvert.SerializeObject(paramValue), Encoding.UTF8, "application/json");
                    }
                    isBody = true;
                } else if (header_arg != null) {
                    if (paramValue is string headerValue) {
                        client.DefaultRequestHeaders.Add(paramName, headerValue);
                    } else if (paramValue is IEnumerable<string> headerValues) {
                        client.DefaultRequestHeaders.Add(paramName, headerValues);
                    } else {
                        throw new NotSupportedException();
                    }
                } else {
                    throw new ArgumentNullException(param.Name);
                }
            }
            #endregion

            var requestMessage = new HttpRequestMessage(
                method,
                BuildUri(
                    route,
                    (httpGet as HttpMethodAttribute ??
                     httpPost as HttpMethodAttribute ??
                     httpPut as HttpMethodAttribute ??
                     httpDelete as HttpMethodAttribute),
                    routeOrQueryArgs));

            if (method == HttpMethod.Get || method == HttpMethod.Delete) {

            } else {
                requestMessage.Content = bodyContent ?? formContent;
            }

            var response = await client.SendAsync(requestMessage);

            if (invocation.Method.ReturnType == typeof(void)) {
                return;
            } else if (invocation.Method.ReturnType == typeof(Task)) {
                invocation.ReturnValue = Task.Run(() => { });
                return;
            } else if (invocation.Method.ReturnType.IsGenericType &&
                 invocation.Method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>)) {
                var result = JsonConvert.DeserializeObject(
                       await response.Content.ReadAsStringAsync(),
                       invocation.Method.ReturnType.GetGenericArguments()[0]);
                invocation.ReturnValue = typeof(Task).GetMethod("FromResult")
                    .InvokeGeneric(invocation.Method.ReturnType.GetGenericArguments()[0].BoxingToArray(),
                    result.BoxingToArray());
            } else {
                invocation.ReturnValue =
                   JsonConvert.DeserializeObject(
                       await response.Content.ReadAsStringAsync(),
                       invocation.Method.ReturnType);
            }
        }

        public void Intercept(IInvocation invocation) {
            InterceptAsync(invocation).ToSync();
        }
    }
}
