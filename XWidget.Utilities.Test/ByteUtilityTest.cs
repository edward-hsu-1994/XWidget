using MathUtility.Utilities.Test.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace XWidget.Utilities.Test {
    public class ByteUtilityTest {
        [Fact(DisplayName = "ByteUtility.ToHexAndFromHex")]
        public void ToHexAndFromHex() {
            Random rand = new Random((int)DateTime.Now.Ticks);

            var testData = new byte[1024];

            rand.NextBytes(testData);

            Assert.Equal(testData, ByteUtility.FromHex(ByteUtility.ToHex(testData)));
        }
    }
}
