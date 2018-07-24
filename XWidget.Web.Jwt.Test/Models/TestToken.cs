using System;
using System.Collections.Generic;
using System.Text;

namespace XWidget.Web.Jwt.Test.Models {
    public class TestToken : IJwtToken<DefaultJwtHeader, CommonPayload> {
        public DefaultJwtHeader Header { get; set; }
        public CommonPayload Payload { get; set; }
    }
}
