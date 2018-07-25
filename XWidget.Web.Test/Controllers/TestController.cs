using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace XWidget.Web.Test.Controllers {
    [Route("api/[controller]")]
    public class TestController : Controller {
        public Nullable<bool> TestValue { get; set; }
        public TestController(Nullable<bool> value) {
            TestValue = value;
        }
        [HttpGet]
        public string Test() {
            Assert.NotNull(this.TestValue);
            return string.Empty;
        }
    }
}
