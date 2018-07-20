using XWidget.Extensions.Test.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace XWidget.Extensions.Test {
    public class RandomExtensionTest {
        [Theory(DisplayName = "RandomExtension.NextDouble(min,max)")]
        [InlineData(1, 25)]
        [InlineData(23, 25)]
        [InlineData(0, 9)]
        public void NextDouble_Min_Max(int min, int max) {
            Random rand = new Random((int)DateTime.Now.Ticks);
            var value = rand.NextDouble(min, max);
            Assert.True(value >= min && value < max);
        }

        [Theory(DisplayName = "RandomExtension.NextDouble(max)")]
        [InlineData(25)]
        [InlineData(1)]
        [InlineData(8)]
        public void NextDouble_Max(int max) {
            Random rand = new Random((int)DateTime.Now.Ticks);
            var value = rand.NextDouble(max);
            Assert.True(value < max);
        }

        [Fact(DisplayName = "RandomExtension.NextBool")]
        public void NextDouble_NextBool() {
            Random rand = new Random((int)DateTime.Now.Ticks);
            var ary = Enumerable.Range(0, 100).Select(x => rand.NextBool()).ToArray();
            Assert.True(ary.Any(x => x) && ary.Any(x => !x));
        }

        [Fact(DisplayName = "RandomExtension.NextEnum")]
        public void NextDouble_NextEnum() {
            Random rand = new Random((int)DateTime.Now.Ticks);
            var ary = Enumerable.Range(0, 300).Select(x => rand.NextEnum<OSType>()).ToArray();
            Assert.True(
                ary.Any(x => x == OSType.Windows) &&
                ary.Any(x => x == OSType.Linux) &&
                ary.Any(x => x == OSType.Mac));
        }

        [Fact(DisplayName = "RandomExtension.NextString")]
        public void NextDouble_NextString() {
            Random rand = new Random((int)DateTime.Now.Ticks);
            var ary = Enumerable.Range(0, 50).Select(x => rand.NextString(new string[] { "A", "B" })).ToArray();
            Assert.True(
                ary.Any(x => x == "A") &&
                ary.Any(x => x == "B"));
        }
    }
}
