using System;
using System.Collections.Generic;
using System.Text;

namespace XWidget.Web.SSO {
    public interface ISsoConfiguration {
        string AppId { get; }
        string AppKey { get; }
        List<string> Scopes { get; }
    }
}
