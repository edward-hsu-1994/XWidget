using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;

namespace XWidget.Web.Jwt {
    /// <summary>
    /// 預設JWT格式
    /// </summary>
    public class DefaultJwtToken : IJwtToken<DefaultJwtHeader, JObject> {
        public DefaultJwtToken() {
            Header = new DefaultJwtHeader();
            Payload = new JObject();
        }

        public DefaultJwtHeader Header { get; set; }

        public JObject Payload { get; set; }
    }
}
