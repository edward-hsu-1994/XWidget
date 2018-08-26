using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace XWidget.Utilities.Test {
    public class UriUtilityTest {
        [Fact]
        public void RenderUri() {
            Assert.Equal(
                new Uri("https://example.com/api/A?keyword=a&keyword=b"),
                UriUtility.Render(
                    "https://example.com/api/[controller]",
                    new Dictionary<string, object>() {
                        ["controller"] = "A",
                        ["keyword"] = new string[] { "a", "b" }
                    })
            );

            Assert.Equal(
                new Uri("https://example.com/api/A/B?keyword=a&keyword=b&keyword=c&keyword=d"),
                UriUtility.Render(
                    "https://example.com/api/[controller]/{action}?keyword=a&keyword=b",
                    new Dictionary<string, object>() {
                        ["controller"] = "A",
                        ["action"] = "B",
                        ["keyword"] = new string[] { "c", "d" }
                    })
            );

            Assert.Equal(
                new Uri("https://example.com/api/A/B"),
                UriUtility.Render(
                    "https://example.com/api/[controller]/{action}",
                    new Dictionary<string, object>() {
                        ["controller"] = "A",
                        ["action"] = "B"
                    })
            );

            Assert.Throws<KeyNotFoundException>(() => {
                UriUtility.Render(
                    "https://example.com/api/[controller]/{action}?keyword=a&keyword=b",
                    new Dictionary<string, object>() {
                        ["controller"] = "A",
                        ["keyword"] = new string[] { "c", "d" }
                    });
            });

            Assert.Equal(
                new Uri("https://example.com/api/"),
                UriUtility.Render(
                    "https://example.com/api/[controller?]/",
                    new Dictionary<string, object>() {
                        ["controller"] = null
                    }));

            Assert.Equal(
                new Uri("https://example.com/api/Home"),
                UriUtility.Render(
                    "https://example.com/api/[controller=Home]",
                    new Dictionary<string, object>() {
                        ["controller"] = null
                    }));

            Assert.Throws<ArgumentNullException>(() => {
                UriUtility.Render(
                    "https://example.com/api/[controller]",
                    new Dictionary<string, object>() {
                        ["controller"] = null
                    });
            });
        }
    }
}
