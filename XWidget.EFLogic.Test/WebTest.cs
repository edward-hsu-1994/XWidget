using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace XWidget.EFLogic.Test {
    public class WebTest : TestBase {
        public WebTest(TestWebFactory factory) : base(factory) {
        }

        [Fact]
        public async Task LogicTest() {
            var response1 = await Client.GetAsync("/api/test");

            Assert.True(response1.IsSuccessStatusCode);
        }
    }
}
