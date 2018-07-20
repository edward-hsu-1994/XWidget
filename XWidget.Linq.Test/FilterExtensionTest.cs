using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace XWidget.Linq.Test {
    public class FilterExtensionTest {
        [Fact(DisplayName = "FilterExtensionTest.Filter")]
        public void FilterEnableTest() {
            bool[] test = new bool[] { true, false, true, true };


            Assert.True(test.Filter(x => x, true).All(x => x));
            Assert.True(test.Filter(x => x, false).All(x => !x));
            Assert.False(test.Filter(x => x, null).All(x => x));
        }
    }
}
