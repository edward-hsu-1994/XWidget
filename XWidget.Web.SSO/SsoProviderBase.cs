using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using XWidget.Utilities;

namespace XWidget.Web.SSO {
    public abstract class SsoProviderBase : ISsoProvider {

        public abstract string Name { get; }

        public ISsoConfiguration Configuration { get; set; }

        public SsoProviderBase(ISsoConfiguration config) {
            Configuration = config;
        }

        public abstract Task<string> GetLoginUrlAsync(HttpContext context);

        public string GetCallbackUrl(HttpContext context) {
            var currentPath = string.Concat(
                context.Request.Scheme,
                "://",
                context.Request.Host.ToUriComponent(),
                context.Request.PathBase.ToUriComponent(),
                context.Request.Path.ToUriComponent()
            );
            if (currentPath.EndsWith("/")) {
                currentPath = currentPath.Substring(0, currentPath.Length - 1);
            }

            if (currentPath.EndsWith("-callback", StringComparison.CurrentCultureIgnoreCase)) {

            } else {
                currentPath += "-callback";
            }

            currentPath += context.Request.QueryString.ToUriComponent();

            return currentPath;
        }

        public abstract Task<string> GetLoginCallbackTokenAsync(HttpContext context);

        public abstract Task<bool> VerifyTokenAsync(string token);

        public abstract Task<bool> VerifyCallbackRequest(HttpContext context);

        /// <summary>
        /// 產生狀態碼
        /// </summary>
        /// <returns>狀態碼</returns>
        public virtual string GenerateStateCode() {
            var part1 = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 16);
            var part2 = DateTimeUtility.GetNowUnixTimestamp();

            var head = (part1 + part2).ToUpper();
            var hash = (head + Configuration.AppId + Configuration.AppKey).ToHashString<MD5>();
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(head + hash));
        }

        /// <summary>
        /// 驗證狀態碼
        /// </summary>
        /// <param name="stateCode">狀態碼</param>
        /// <returns>是否合法</returns>
        public virtual bool VerifyStateCode(string stateCode) {
            stateCode = Encoding.UTF8.GetString(Convert.FromBase64String(stateCode));

            var head = stateCode.Substring(0, stateCode.Length - 32);

            if (DateTimeUtility.GetNowUnixTimestamp() - long.Parse(head.Substring(16)) > 60 * 15) {
                return false;
            }

            var hash = stateCode.Substring(stateCode.Length - 32);

            return (head + Configuration.AppId + Configuration.AppKey).ToHashString<MD5>() == hash;
        }
    }
}
