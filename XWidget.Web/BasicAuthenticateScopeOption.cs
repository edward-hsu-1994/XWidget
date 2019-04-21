using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace XWidget.Web {
    public class BasicAuthenticateScopeOption {
        public PathString Path { get; set; }
        public string Realm { get; set; }
    }
}
