using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Xunit;

namespace XWidget.Cryptography.Test {
    public class StreamExtensionTest {
        [Fact(DisplayName = "StreamExtension.ToHashString")]
        public void ToHashString() {
            var path = Guid.NewGuid().ToString() + ".txt";
            using (var stream = System.IO.File.CreateText(path)) {
                stream.Write("1234");
            }

            using (var file = System.IO.File.Open(path, System.IO.FileMode.Open)) {
                Assert.Equal("81dc9bdb52d04dc20036dbd8313ed055", file.ToHashString<MD5>(false));
            }

            System.IO.File.Delete(path);
        }
    }
}
