using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace XWidget.Web.Test {
    public class WebTest {
        [Fact]
        public async Task NoCacheAndUseServiceHelperTest() {
            var webhost = BuildWebHost(new string[0]);

            await webhost.StartAsync();

            var client = new HttpClient();

            var response = await client.GetAsync("http://localhost:9997/api/test");

            Assert.True(response.Headers.CacheControl.NoCache);
            Assert.True(response.Headers.CacheControl.NoStore);

            var response2 = await client.GetStringAsync("http://localhost:9997/gg");

            Assert.Equal("<html>gg</html>", response2);
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseKestrel(options => {
                    options.Listen(IPAddress.Loopback, 9997);
                })
                .UseStartup<Startup>()
                .Build();
    }
}
