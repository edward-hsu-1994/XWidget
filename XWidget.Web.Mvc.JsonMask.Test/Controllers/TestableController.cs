using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using XWidget.Web.Mvc.JsonMask.Test.Models;

namespace XWidget.Web.Mvc.JsonMask.Test.Controllers {
    public class TestableController : Controller {
        public IEnumerable<Category_Controller> TestByAction() {
            return this.Mask(Category_Controller.GetCategories());
        }
    }
}
