using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Http {
    /// <summary>
    /// 針對<see cref="HttpClient"/>的擴充方法
    /// </summary>
    public static class HttpClientExtension {
        /// <summary>
        /// 將 GET 要求傳送至指定的 URI，並透過非同步作業，以Json形式傳回回應內容
        /// </summary>
        /// <param name="client"></param>
        /// <param name="requestUri">傳送要求的目標 URI</param>
        /// <returns>操作結果</returns>
        public static async Task<JToken> GetJsonAsync(this HttpClient client, string requestUri) {
            return JToken.Parse(await client.GetStringAsync(requestUri));
        }

        /// <summary>
        /// 將 GET 要求傳送至指定的 URI，並透過非同步作業，以Json形式傳回回應內容
        /// </summary>
        /// <param name="client"></param>
        /// <param name="requestUri">傳送要求的目標 URI</param>
        /// <returns>操作結果</returns>
        public static async Task<JToken> GetJsonAsync(this HttpClient client, Uri requestUri) {
            return JToken.Parse(await client.GetStringAsync(requestUri));
        }

        /// <summary>
        /// Http Response轉換為Json形式
        /// </summary>
        /// <param name="response">Http Response</param>
        /// <returns>操作結果</returns>
        public static async Task<JToken> ToJsonAsync(this HttpResponseMessage response) {
            return JToken.Parse(await response.Content.ReadAsStringAsync());
        }
    }
}
