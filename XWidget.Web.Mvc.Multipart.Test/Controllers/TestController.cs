using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace XWidget.Web.Mvc.Multipart.Test.Controllers {
    [Route("api/[controller]")]
    public class TestController : Controller {

        public class Name {
            public string First { get; set; }
            public string Last { get; set; }
        }

        public class User {
            [FromJson]
            public Name Name { get; set; }
            public string Address { get; set; }
            public IFormFileCollection Files { get; set; }
        }

        public TestController() {

        }

        [HttpGet]
        public void Get() {

        }

        [HttpPost("a")]
        public void Post1(
            User user
            ) {
            Assert.NotNull(user.Name);
            Assert.NotNull(user.Name.First);
            Assert.NotNull(user.Name.Last);
            Assert.NotNull(user.Files.Count > 0);
        }

        [HttpPost("b")]
        public void Post2(
            [FromJson(Name = "data")]User user,
            [FromForm]IFormFileCollection files
            ) {
            Assert.NotNull(user.Name);
            Assert.NotNull(user.Name.First);
            Assert.NotNull(user.Name.Last);
            Assert.NotNull(files.Count > 0);
        }
    }
}