using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace XWidget.Rest.Test.Models {
    [Route("https://jsonplaceholder.typicode.com/")]
    public abstract class TestClient {
        [HttpGet("posts")]
        public abstract Post[] GetPosts([FromQuery]int? userId = null);

        [HttpGet("posts/{id}")]
        public abstract Post GetPost([FromRoute(Name = "id")]int postId);

        [HttpPost("posts")]
        public abstract Post CreatePost([FromBody]Post post);

        [HttpPut("posts/{id}")]
        public abstract Task<Post> UpdatePost([FromRoute(Name = "id")]int postId, [FromBody]Post post);

        [HttpDelete("posts/{id}")]
        public abstract Task DeletePost([FromRoute(Name = "id")]int postId);
    }
}
