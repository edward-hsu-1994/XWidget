using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using XWidget.EFLogic.Test.Logic;
using XWidget.EFLogic.Test.Models;

namespace XWidget.EFLogic.Test.Controllers {
    [Route("api/[controller]")]
    public class TestController : ControllerBase {
        public TestLogicManager Manager { get; set; }
        public TestController(TestLogicManager manager) {
            Manager = manager;
        }

        [HttpGet]
        public async void Test() {
            var categoryLogic = Manager.GetLogicByType<Category, Guid>();

            var category = categoryLogic.Create(new Category() {
                Name = "Test01"
            });

            category = categoryLogic.Update(new Category() {
                Id = category.Id,
                Name = "Test02"
            });

            Assert.Equal("Test02", category.Name);

            category = categoryLogic.Get(category.Id);

            Assert.Equal("Test02", category.Name);

            Assert.Equal(1, categoryLogic.List(x => x.Id == category.Id).Count());

            categoryLogic.Delete(category.Id);

            Assert.Equal(0, categoryLogic.List(x => x.Id == category.Id).Count());

            var category2 = categoryLogic.Create(new Category() {
                Name = "Test03"
            });

            Assert.NotEmpty(Manager.Search<Category>("Test%", x => x.Name));

            Assert.NotEmpty(Manager.Search<Category>("Test%"));

            Assert.NotEmpty(categoryLogic.List(x => x.Notes.Count > 0));

            var noteLogic = Manager.GetLogicByType<Note, Guid>();

            Assert.NotEmpty(noteLogic.List());

            var deleteTargets = categoryLogic.List(x => x.Notes.Count > 0).ToArray();
            foreach (var cate in deleteTargets) {
                if (categoryLogic.Exists(cate.Id) && Manager.Exists<Category>(cate.Id)) {
                    Manager.Delete<Category>(cate.Id);
                }
            }

            Assert.Empty(noteLogic.List());

            Assert.Empty(categoryLogic.List(x => x.Notes.Count > 0));


            categoryLogic.Create(new Category() { Name = "AAAA" });
            categoryLogic.Create(new Category() { Name = "BBBB" });

            Manager.BatchDeleteRange<Category, Guid>(categoryLogic.List().Select(x => x.Id));

            Assert.Empty(categoryLogic.List());

            categoryLogic.Create(new Category() { Name = "AAAA" });
            categoryLogic.Create(new Category() { Name = "BBBB" });

            Manager.BatchDeleteRange<Guid>(typeof(Category), categoryLogic.List().Select(x => x.Id));

            Assert.Empty(categoryLogic.List());
        }
    }
}
