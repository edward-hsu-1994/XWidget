using System;
using System.Collections.Generic;
using System.Text;

namespace XWidget.Web.SSO {
    /// <summary>
    /// 預設SSO設定
    /// </summary>
    public class DefaultSsoConfiguration : ISsoConfiguration {
        /// <summary>
        /// 應用程式唯一識別號
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string AppKey { get; set; }

        public List<string> Scopes { get; set; } = new List<string>();
    }
}
