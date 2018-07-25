using XWidget.Reflection.Test.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace XWidget.Reflection.Test {
    public class AccessExpressionHelperTest {
        [Fact(DisplayName = "AccessExpressionHelper.CreateAccessFunc<T>")]
        public void CreateAccessFunc() {
            var func = AccessExpressionUtility.CreateAccessFunc<TestClass>("Value2");
            var func2 = AccessExpressionUtility.CreateAccessFunc<TestClass>("Value3");

            var instance = new TestClass();
            Assert.Equal(instance.Value2, func.Compile()(instance));
            Assert.Equal(instance.Value3, func2.Compile()(instance));
        }

        [Fact(DisplayName = "AccessExpressionHelper.CreateAccessFunc<T,R>")]
        public void CreateAccessFunc2() {
            var func = AccessExpressionUtility.CreateAccessFunc<TestClass, string>("Value2");
            var func2 = AccessExpressionUtility.CreateAccessFunc<TestClass, string>("Value3");

            var instance = new TestClass();
            Assert.Equal(instance.Value2, func(instance));
            Assert.Equal(instance.Value3, func2(instance));
        }
    }
}
