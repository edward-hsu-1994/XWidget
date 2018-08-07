using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Reflection;
using Xunit;
using XWidget.Web.Mvc.PropertyMask.Test.Controllers;
using XWidget.Web.Mvc.PropertyMask.Test.Models;

namespace XWidget.Web.Mvc.PropertyMask.Test {
    public class PropertyMaskTest {
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
        /// 依據包裝類型屏蔽
        /// </summary>
        [Fact]
        public void ByPackageType() {
            var maskedResult = ControllerExtension.Mask(null, Category_PackageType.GetCategories());

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

        /// <summary>
        /// 循環參考測試
        /// </summary>
        [Fact]
        public void RefLoopTest() {
            var obj = new RefLoop();
            obj.Loop = obj;

            var maskedResult = ControllerExtension.Mask(null, obj, "NoPatternName");
        }

        /// <summary>
        /// EFCore的測試
        /// </summary>
        [Fact]
        public void EFTest() {
            var options = new DbContextOptionsBuilder<TestContext>()
                .UseInMemoryDatabase(databaseName: "Find_searches_url")
                .Options;

            using (var context = new TestContext(options)) {
                Category_EF category_EF1;
                context.Categories.Add(category_EF1 = new Category_EF { Name = "A" });

                category_EF1.Children.Add(new Category_EF() {
                    Name = "A-1"
                });

                context.Categories.Add(new Category_EF { Name = "B" });
                context.Categories.Add(new Category_EF { Name = "C" });
                context.SaveChanges();
            }

            using (var context = new TestContext(options)) {
                var data = ControllerExtension.Mask(
                    null,
                    context.Categories.Where(x => 1 == 1),
                    "Mask");

                foreach (var category in data) {
                    Assert.Null(category.Children);
                    Assert.Null(category.Parent);
                }

                //嘗試儲存變更，確認是否deepclone有作用
                context.SaveChanges();
            }

            using (var context = new TestContext(options)) {
                Assert.True(context.Categories.Any(x => x.Children.Count > 0));
            }
        }
    }
}
