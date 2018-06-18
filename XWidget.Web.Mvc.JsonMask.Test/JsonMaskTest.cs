using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using System;
using System.Linq;
using System.Reflection;
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
        /// 依據定義類型屏蔽
        /// </summary>
        [Fact]
        public void ByDeclaringType() {
            var maskedResult = ControllerExtension.Mask(null, Category_DeclaringType.GetCategories());

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

        /// <summary>
        /// 依據操作或方法屏蔽
        /// </summary>
        [Fact]
        public void ByAction() {
            var controller = new TestableController();

            var actionContext = new ActionContext(
                new DefaultHttpContext(),
                new RouteData(),
                new ControllerActionDescriptor() {
                    ActionName = nameof(TestableController.TestByAction),
                    ControllerName = nameof(TestableController),
                    ControllerTypeInfo = typeof(TestableController).GetTypeInfo()
                });

            controller.ControllerContext = new ControllerContext(actionContext);

            foreach (var category in controller.TestByAction()) {
                Assert.Null(category.Children);
            }
        }

        /// <summary>
        /// 依據操作或方法回傳類型屏蔽
        /// </summary>
        [Fact]
        public void ByReturnType() {
            var controller = new TestableController();

            var actionContext = new ActionContext(
                new DefaultHttpContext(),
                new RouteData(),
                new ControllerActionDescriptor() {
                    ActionName = nameof(TestableController.TestByActionReturnType),
                    ControllerName = nameof(TestableController),
                    ControllerTypeInfo = typeof(TestableController).GetTypeInfo()
                });

            controller.ControllerContext = new ControllerContext(actionContext);
            controller.ControllerContext.ActionDescriptor = new ControllerActionDescriptor() {
                MethodInfo = typeof(TestableController).GetMethod(nameof(TestableController.TestByActionReturnType))
            };
            foreach (var category in controller.TestByActionReturnType()) {
                Assert.Null(category.Children);
            }
        }
    }
}
