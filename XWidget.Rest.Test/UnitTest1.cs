using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;
using XWidget.Rest.Test.Models;

namespace XWidget.Rest.Test {
    public class UnitTest1 {
        [Fact]
        public async Task Test1() {
            var client = new RestClientBuilder<TestClient>()
                .Build();

            return;

            var test = client.GetPosts();

            Assert.Equal(100, test.Length);



            Post newPost = new Post() {
                title = "foo",
                userId = 1,
                body = "bar"
            };

            Post createResult = client.CreatePost(newPost);
            newPost.id = createResult.id;

            Assert.Equal(JsonConvert.SerializeObject(newPost), JsonConvert.SerializeObject(createResult));

            Post firstPost = client.GetPost(1);

            firstPost.title = "aaaa";
            firstPost.body = "bbbb";

            Post updatedPost = await client.UpdatePost(1, firstPost);

            Assert.Equal(JsonConvert.SerializeObject(firstPost), JsonConvert.SerializeObject(updatedPost));

            await client.DeletePost(1);
        }
    }
}
