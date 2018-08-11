using XWidget.Reflection.Test.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace XWidget.Reflection.Test {
    public class ExpandoObjectUtilityTest {
        [Fact(DisplayName = "ExpandoObjectUtilityTest.ConvertToExpando")]
        public void ConvertToExpando() {
            var obj = new TestClass();
            dynamic test = ExpandoObjectUtility.ConvertToExpando(obj);

            Assert.Equal(obj.Value2, test.Value2);
            Assert.Equal(obj.Value3, test.Value3);
        }
    }
}
