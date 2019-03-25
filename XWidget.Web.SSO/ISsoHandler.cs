using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace XWidget.Web.SSO {
    /// <summary>
    /// SSO處理介面
    /// </summary>
    public interface ISsoHandler {
        /// <summary>
        /// 使用第三方登入
        /// </summary>
        /// <param name="provider">SSO提供者</param>
        /// <param name="token">存取權杖</param>
        /// <param name="context">HttpContext</param>
        /// <returns></returns>
        Task OnLogin(ISsoProvider provider, string token, HttpContext context);

        /// <summary>
        /// 第三方登入失敗
        /// </summary>
        /// <param name="provider">SSO提供者</param>
        /// <param name="context">HttpContext</param>
        /// <returns></returns>
        Task OnError(ISsoProvider provider, HttpContext context);
    }
}
