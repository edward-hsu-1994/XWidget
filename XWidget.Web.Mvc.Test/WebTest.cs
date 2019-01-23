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
using XWidget.Linq;
using XWidget.Web.Exceptions;

namespace XWidget.Web.Mvc.Test {
    public class WebTest {
        [Fact]
        public async Task ExceptionAndPagingTest() {
            var webhost = BuildWebHost(new string[0]);

            await webhost.StartAsync();

            var client = new HttpClient();

            var response1 = await client.GetAsync("http://localhost:9996/api/test");

            Assert.False(response1.IsSuccessStatusCode);

            var response1Content = JObject.Parse(await response1.Content.ReadAsStringAsync()).ToObject<UnknowException>();

            Assert.Equal(response1Content.Name, "未知錯誤");

            var response2 = await client.GetAsync("http://localhost:9996/api/test/list");

            Assert.True(response2.IsSuccessStatusCode);

            var response2ContextJObject = JObject.Parse(await response2.Content.ReadAsStringAsync());

            Assert.Equal(response2ContextJObject["result"].Count(), 10);

            var response3 = await client.GetAsync("http://localhost:9996/api/test/clientPost");

            Assert.True(response3.IsSuccessStatusCode);

            var responseText = await response3.Content.ReadAsStringAsync();

            Assert.Contains("https://example.com/", responseText);
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
