using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace XWidget.Web.Mvc.Test.Controllers {
    [Route("api/[controller]")]
    public class TestController : ControllerBase {
        [HttpGet]
        public string Test() {
            throw new Exception("Test Error");
        }

        [HttpGet("list")]
        public PaginationResult<IEnumerable<int>> List() {
            return Paging(Enumerable.Range(0, 100), 0, 10);
        }
    }
}
