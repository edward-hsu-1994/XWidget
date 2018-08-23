using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace XWidget.Extensions.Test {
    public class HttpClientExtensionTest {
        [Fact(DisplayName = "HttpClientExtension.GetJsonAsync")]
        public async Task GetJsonAsync() {
            HttpClient client = new HttpClient();
            var result1 = await client.GetJsonAsync("https://jsonplaceholder.typicode.com/posts/1");

            var result2 = await client.GetJsonAsync(new Uri("https://jsonplaceholder.typicode.com/posts/1"));
        }

        [Fact(DisplayName = "HttpClientExtension.GetJsonAsync")]
        public async Task ToJsonAsync() {
            HttpClient client = new HttpClient();
            var result = await client.PostAsync("https://jsonplaceholder.typicode.com/posts", new StringContent("none"));
            var jsonResult = await result.ToJsonAsync();
        }
    }
}
