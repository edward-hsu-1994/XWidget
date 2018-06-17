using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using XWidget.Web.Mvc.JsonMask.Test.Models;

namespace XWidget.Web.Mvc.JsonMask.Test.Controllers {
    public class TestableController : Controller {
        //[HttpGet("TestByController")]

        public object Result { get; set; }

        public override void OnActionExecuting(ActionExecutingContext context) {
            base.OnActionExecuting(context);
        }

        public void TestByController() {
            Result = this.Mask(Category_Controller.GetCategories());
        }
    }
}
