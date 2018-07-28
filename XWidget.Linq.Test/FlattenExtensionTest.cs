using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using XWidget.Linq.Test.Models;

namespace XWidget.Linq.Test {
    public class FlattenExtensionTest {
        [Fact(DisplayName = "FlattenExtensionTest.Flatten")]
        public void Flatten() {
            var paths = Category.GetTestSample().Flatten(x => x.Children);

            Assert.Equal(5, paths.Count());
        }

        [Fact(DisplayName = "FlattenExtensionTest.FlattenMany")]
        public void FlattenMany() {
            var paths = Category.GetTestSample().FlattenMany(x => x.Children);

            Assert.Equal(8, paths.Count());
        }
    }
}
