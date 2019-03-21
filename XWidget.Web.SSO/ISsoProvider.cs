using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace XWidget.Web.SSO {
    /// <summary>
    /// SSO登入提供者
    /// </summary>
    public interface ISsoProvider {
        /// <summary>
        /// 名稱
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 設定
        /// </summary>
        ISsoConfiguration Configuration { get; }

        /// <summary>
        /// 產生對應第三方SSO登入網址
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <returns>網址</returns>
        Task<string> GetLoginUrlAsync(HttpContext context);

        /// <summary>
        /// 驗證第三方登入回呼請求合法性
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <returns>是否合法</returns>
        Task<bool> VerifyCallbackRequest(HttpContext context);

        /// <summary>
        /// 取得第三方登入回呼請求中帶有的Token
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <returns>Token</returns>
        Task<string> GetLoginCallbackTokenAsync(HttpContext context);

        /// <summary>
        /// 驗證第三方登入取得的Token合法性
        /// </summary>
        /// <param name="token">Token</param>
        /// <returns>Token是否合法</returns>
        Task<bool> VerifyTokenAsync(string token);
    }
}
