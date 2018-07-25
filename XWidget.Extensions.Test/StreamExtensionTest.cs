using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace XWidget.Extensions.Test {
    public class StreamExtensionTest {
        [Fact(DisplayName = "StreamExtension.ToBytes")]
        public void ToBytes() {
            MemoryStream stream = new MemoryStream();
            for (byte i = 0; i <= 100; i++) {
                stream.WriteByte(i);
            }
            stream.Flush();

            var binary = stream.ToBytes();
            for (byte i = 0; i <= 100; i++) {
                Assert.Equal(binary[i], i);
            }
        }

        [Fact(DisplayName = "StreamExtension.ToStream")]
        public void ToStream() {
            byte[] binary = new byte[100];
            Random rand = new Random((int)DateTime.Now.Ticks);
            rand.NextBytes(binary);
            Assert.Equal(binary.ToStream().ToBytes(), binary);
        }
    }
}
