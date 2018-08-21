using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace XWidget.Linq.Test {
    public class TestRefType {
        public bool Value { get; set; }
        public TestRefType(bool value) {
            Value = value;
        }
    }
    public class FilterExtensionTest {
        [Fact(DisplayName = "FilterExtensionTest.Filter")]
        public void FilterEnableTest() {
            bool[] test = new bool[] { true, false, true, true };


            Assert.True(test.Filter(x => x, true).All(x => x));
            Assert.True(test.Filter(x => x, false).All(x => !x));
            Assert.False(test.Filter(x => x, null).All(x => x));

            Assert.True(test.Select(x => new TestRefType(x)).AsQueryable().Filter(x => x.Value, (bool?)true).All(x => x.Value));
            Assert.True(test.Select(x => new TestRefType(x)).AsQueryable().Filter(x => x.Value, (bool?)false).All(x => !x.Value));
            Assert.False(test.Select(x => new TestRefType(x)).AsQueryable().Filter(x => x.Value, null).All(x => x.Value));
        }
    }
}
