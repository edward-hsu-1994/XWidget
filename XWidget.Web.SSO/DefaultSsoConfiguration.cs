using System;
using System.Collections.Generic;
using System.Text;

namespace XWidget.Web.SSO {
    public class DefaultSsoConfiguration : ISsoConfiguration {
        public string AppId { get; set; }

        public string AppKey { get; set; }

        public List<string> Scopes { get; set; } = new List<string>();
    }
}
