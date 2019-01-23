using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using XWidget.Linq;

namespace XWidget.Web.Mvc.Test.Controllers {
    [Route("api/[controller]")]
    public class TestController : ControllerBase {
        [HttpGet]
        public string Test() {
            throw new Exception("Test Error");
        }

        [HttpGet("list")]
        public Paging<int> List() {
            return Paging(Enumerable.Range(0, 100), 0, 10);
        }

        [HttpGet("clientPost")]
        public IActionResult clientPost() {
            return this.RedirectWithClientPostFormData("https://example.com/", new Dictionary<string, string>() { ["id"] = "1234" });
        }
    }
}
