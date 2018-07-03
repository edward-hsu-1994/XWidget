using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace XWidget.Web.Mvc.Multipart.WebTest.Controllers {
    [Route("api/[controller]")]
    public class ValuesController : Controller {

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
        [HttpPost("a")]
        public void Post1(
            User user
            ) {

        }

        [HttpPost("b")]
        public void Post2(
            [FromJson(Name = "data")]User user,
            [FromForm]IFormFileCollection files
            ) {

        }
    }
}
