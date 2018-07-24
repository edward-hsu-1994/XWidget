using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace XWidget.Web.Mvc.Test {
    public class WebTest {
        [Fact]
        public async Task ExceptionAndPagingTest() {
            var webhost = BuildWebHost(new string[0]);

            await webhost.StartAsync();

            var client = new HttpClient();

            var response1 = await client.GetAsync("http://localhost:9996/api/test");

            Assert.False(response1.IsSuccessStatusCode);

            var response2 = await client.GetAsync("http://localhost:9996/api/test/list");

            Assert.True(response2.IsSuccessStatusCode);

            var response2Content = JObject.Parse(await response2.Content.ReadAsStringAsync()).ToObject<PaginationResult<IEnumerable<int>>>();

            Assert.Equal(response2Content.Result.Count(), 10);
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseKestrel(options => {
                    options.Listen(IPAddress.Loopback, 9996);
                })
                .UseStartup<Startup>()
                .Build();
    }
}
