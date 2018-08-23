using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace XWidget.Extensions.Test {
    public class ByteExtensionTest {
        [Fact(DisplayName = "ByteExtension.ToStream")]
        public void ToStream() {
            var data = new byte[] { 0, 1, 2, 3, 4 };

            var stream = data.ToStream();
            var stream2 = new MemoryStream(data);

            for (int i = 0; i < stream2.Length; i++) {
                Assert.Equal(stream.ReadByte(), stream2.ReadByte());
            }
        }

        [Fact(DisplayName = "ByteExtension.ToBytes")]
        public void ToBytes() {
            var data = new byte[] { 4, 3, 2, 1, 0 };

            var arys = new MemoryStream(new byte[] { 4, 3, 2, 1, 0 }).ToBytes();

            for (int i = 0; i < data.Length; i++) {
                Assert.Equal(data[i], arys[i]);
            }
        }

        [Fact(DisplayName = "ByteExtension.ToStreamAndToBytes")]
        public void ToStreamAndToBytes() {
            var data = new byte[] { 0, 1, 2, 3, 4 };

            var arys = data.ToStream().ToBytes();

            for (int i = 0; i < data.Length; i++) {
                Assert.Equal(data[i], arys[i]);
            }
        }

        [Fact(DisplayName = "ByteExtension.ToHex")]
        public void ToHex() {
            var data = new byte[] { 0, 1, 2, 3, 4 };

            var dataSegments = data.ToHex()
                .Split<char>(2)
                .Select(x => new string(x.ToArray()))
                .Select(x => byte.Parse(x, System.Globalization.NumberStyles.HexNumber));

            Assert.Equal(data, dataSegments);
        }
    }
}
