using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace XWidget.Linq.Test {
    public class MaxMinExtensionTest {
        [Fact(DisplayName = "MaxMinExtensionTest.MaxOrDefault")]
        public void MaxOrDefault() {
            Assert.Equal(100, Enumerable.Range(1, 100).MaxOrDefault(x => x));

            Assert.Equal(default(int), Enumerable.Range(1, 0).MaxOrDefault(x => x));

            Assert.Null(Enumerable.Range(1, 0).MaxOrDefault(x => x, null));
        }

        [Fact(DisplayName = "MaxMinExtensionTest.MinOrDefault")]
        public void MinOrDefault() {
            Assert.Equal(1, Enumerable.Range(1, 100).MinOrDefault(x => x));

            Assert.Equal(default(int), Enumerable.Range(1, 0).MaxOrDefault(x => x));

            Assert.Null(Enumerable.Range(1, 0).MinOrDefault(x => x, null));
        }
    }
}
