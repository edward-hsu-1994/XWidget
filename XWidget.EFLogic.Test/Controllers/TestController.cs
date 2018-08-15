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

            var category = await categoryLogic.CreateAsync(new Category() {
                Name = "Test01"
            });

            category = await categoryLogic.UpdateAsync(new Category() {
                Id = category.Id,
                Name = "Test02"
            });

            Assert.Equal("Test02", category.Name);

            category = await categoryLogic.GetAsync(category.Id);

            Assert.Equal("Test02", category.Name);

            Assert.Equal(1, (await categoryLogic.ListAsync(x => x.Id == category.Id)).Count());

            await categoryLogic.DeleteAsync(category.Id);

            Assert.Equal(0, (await categoryLogic.ListAsync(x => x.Id == category.Id)).Count());

            var category2 = await categoryLogic.CreateAsync(new Category() {
                Name = "Test03"
            });

            Assert.NotEmpty(await categoryLogic.SearchAsync("Test%", x => x.Name));

            Assert.NotEmpty(await categoryLogic.SearchAsync("Test%"));
        }
    }
}
