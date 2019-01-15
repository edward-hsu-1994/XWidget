using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace XWidget.Web.AddThis.Test {
    public class WebTest {
        [Fact]
        public async Task UseGoogleAnalyticsTest() {
            var webhost = BuildWebHost(new string[0]);

            await webhost.StartAsync();

            var client = new HttpClient();

            var response = await client.GetStringAsync("http://localhost:9994/");

            Assert.Contains("ra-5c3de839571af158", response);
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseKestrel(options => {
                    options.Listen(IPAddress.Loopback, 9994);
                })
                .UseStartup<Startup>()
                .Build();
    }
}
