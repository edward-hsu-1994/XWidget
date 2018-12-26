using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Xunit;

namespace XWidget.EFLogic.Test {

    public class TestWebFactory : WebApplicationFactory<Startup> {
        protected override IWebHostBuilder CreateWebHostBuilder() {
            return WebHost.CreateDefaultBuilder().UseStartup<Startup>();
        }
    }
    public class TestBase : IClassFixture<TestWebFactory> {
        public HttpClient Client { get; set; }
        public TestBase(TestWebFactory factory) {
            Client = factory.CreateClient();
        }
    }
}
