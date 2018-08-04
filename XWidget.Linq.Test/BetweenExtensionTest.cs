using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace XWidget.Linq.Test {
    public class BetweenExtensionTest {
        [Fact(DisplayName = "BetweenExtensionTest.Between.MaxOnly")]
        public void MaxOnly() {
            Assert.Equal(50, Enumerable.Range(1, 100).Between(x => x, null, 50).Count());
        }

        [Fact(DisplayName = "BetweenExtensionTest.Between.MaxOnly")]
        public void MinOnly() {
            Assert.Equal(50, Enumerable.Range(1, 100).Between(x => x, 51, null).Count());
        }

        [Fact(DisplayName = "BetweenExtensionTest.Between")]
        public void MinAndMax() {
            Assert.Equal(11, Enumerable.Range(1, 100).Between(x => x, 20, 30).Count());
        }

        [Fact(DisplayName = "BetweenExtensionTest.BetweenExpression.MaxOnly")]
        public void ExpressionMaxOnly() {
            Assert.Equal(50, Enumerable.Range(1, 100).AsQueryable().Between(x => x, null, 50).Count());
        }

        [Fact(DisplayName = "BetweenExtensionTest.BetweenExpression.MaxOnly")]
        public void ExpressionMinOnly() {
            Assert.Equal(50, Enumerable.Range(1, 100).AsQueryable().Between(x => x, 51, null).Count());
        }

        [Fact(DisplayName = "BetweenExtensionTest.BetweenExpression")]
        public void ExpressionMinAndMax() {
            Assert.Equal(11, Enumerable.Range(1, 100).AsQueryable().Between(x => x, 20, 30).Count());
        }
    }
}
