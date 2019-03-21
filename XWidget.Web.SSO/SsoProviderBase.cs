using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace XWidget.Web.SSO {
    public abstract class SsoProviderBase<TConfig>
        where TConfig : ISsoConfiguration {

        public abstract string Name { get; }

        public TConfig Configuration { get; set; }

        public SsoProviderBase(TConfig config) {
            Configuration = config;
        }

        public abstract Task<string> GetLoginUrl(HttpContext context);

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

            currentPath += "-callback";

            currentPath += context.Request.QueryString.ToUriComponent();

            return currentPath;
        }

        public abstract Task<string> GetLoginCallbackToken(HttpContext context);

        public abstract Task<bool> VerifyToken(string token);

        /// <summary>
        /// 產生狀態碼
        /// </summary>
        /// <returns>狀態碼</returns>
        public virtual string GenerateStateCode() {
            var part1 = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 16);
            var part2 = DateTime.Now.Ticks;

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
            var hash = stateCode.Substring(stateCode.Length - 32);

            return (head + Configuration.AppId + Configuration.AppKey).ToHashString<MD5>() == hash;
        }
    }
}
