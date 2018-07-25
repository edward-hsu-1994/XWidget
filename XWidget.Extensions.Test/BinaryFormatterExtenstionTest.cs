using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Xunit;

namespace XWidget.Extensions.Test {
    public class BinaryFormatterExtenstionTest {
        [Fact(DisplayName = "BinaryFormatterExtenstion.SerializeAndDeserialize")]
        public void SerializeAndDeserialize() {
            var formatter = new BinaryFormatter();

            var obj = "abc123";

            Assert.Equal(obj, formatter.Deserialize<string>(formatter.Serialize(obj)));
        }
    }
}
