using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using System;
using System.Linq;
using Xunit;
using XWidget.Web.Mvc.JsonMask.Test.Controllers;
using XWidget.Web.Mvc.JsonMask.Test.Models;

namespace XWidget.Web.Mvc.JsonMask.Test {
    public class JsonMaskTest {
        /// <summary>
        /// 依據模式名稱屏蔽
        /// </summary>
        [Fact]
        public void ByPatternName() {
            var maskedResult = ControllerExtension.Mask(null, Category_PatternName.GetCategories(), "MaskName");

            foreach (var category in maskedResult) {
                Assert.Null(category.Children);
            }
        }

        /// <summary>
        /// 依據控制器屏蔽
        /// </summary>
        [Fact]
        public void ByController() {
            var controller = new TestableController();

            var maskedResult = controller.Mask(Category_Controller.GetCategories());

            foreach (var category in maskedResult) {
                Assert.Null(category.Children);
            }
        }
    }
}
