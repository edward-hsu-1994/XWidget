using System;
using Xunit;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using XWidget.Web.Mvc.Multipart.Test.Controllers;
using System.Net;

namespace XWidget.Web.Mvc.Multipart.Test {
    public class MultipartTest {
        [Fact]
        public async Task FormDataPostTest() {
            var webhost = BuildWebHost(new string[0]);

            await webhost.StartAsync();

            var client = new HttpClient();

            var formData1 = new MultipartFormDataContent();
            formData1.Add(new StreamContent(new MemoryStream()), "files", "test.txt");
            formData1.Add(new StringContent("{\"first\":\"firstName\", \"last\":\"lastName\"}"), "name");

            var response1 = await client.PostAsync("http://127.0.0.1:9998/api/test/a", formData1);

            var formData2 = new MultipartFormDataContent();
            formData2.Add(new StreamContent(new MemoryStream()), "files", "test.txt");
            formData2.Add(new StringContent("{\"name\":{\"first\":\"firstName\", \"last\":\"lastName\"}}"), "data");

            var response2 = await client.PostAsync("http://127.0.0.1:9998/api/test/b", formData2);

            await webhost.StopAsync();

            Assert.True(response1.IsSuccessStatusCode);
            Assert.True(response2.IsSuccessStatusCode);
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseKestrel(options => {
                    options.Listen(IPAddress.Loopback, 9998);
                })
                .UseStartup<Startup>()
                .Build();
    }
}
