using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace XWidget.EFLogic.Test {
    public class WebTest {

        [Fact]
        public async Task LogicTest() {
            var webhost = BuildWebHost(new string[0]);

            await webhost.StartAsync();

            var client = new HttpClient();

            var response1 = await client.GetAsync("http://localhost:9995/api/test");

            Assert.True(response1.IsSuccessStatusCode);
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseKestrel(options => {
                    options.Listen(IPAddress.Loopback, 9995);
                })
                .UseStartup<Startup>()
                .Build();
    }
}
