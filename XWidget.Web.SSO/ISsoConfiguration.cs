using System;
using System.Collections.Generic;
using System.Text;

namespace XWidget.Web.SSO {
    /// <summary>
    /// 基礎SSO設定
    /// </summary>
    public interface ISsoConfiguration {
        /// <summary>
        /// 應用程式唯一識別號
        /// </summary>
        string AppId { get; }

        /// <summary>
        /// 應用程式鑰匙
        /// </summary>
        string AppKey { get; }

        /// <summary>
        /// 存取範圍
        /// </summary>
        List<string> Scopes { get; }
    }
}
