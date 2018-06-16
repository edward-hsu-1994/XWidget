using Microsoft.AspNetCore.Mvc;
using System;
using Xunit;
using XWidget.Web.Mvc.JsonMask.Test.Models;

namespace XWidget.Web.Mvc.JsonMask.Test {
    public class JsonMaskTest {
        [Fact]
        public void ByPattern() {
            var maskedResult = ControllerExtension.Mask(null, Category.GetCategories(), "MaskName");

            foreach (var category in maskedResult) {
                Assert.Null(category.Name);
            }
        }

        [Fact]
        public void ByDeclaringType() {
            var maskedResult = ControllerExtension.Mask(null, Category.GetCategories());

            foreach (var category in maskedResult) {
                Assert.Null(category.Children);
            }
        }
    }
}
