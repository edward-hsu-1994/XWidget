using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace XWidget.Extensions.Test {
    public class EnumExtensionTest {
        [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
        public class TestAttr1 : Attribute { }
        public class TestAttr2 : Attribute { }
        public enum TestEnum {
            [TestAttr1]
            [TestAttr1]
            A,
            [TestAttr2]
            B,
            C,
            D
        }

        [Fact(DisplayName = "EnumExtension.GetEnumName")]
        public void GetEnumName() {
            Assert.Equal(TestEnum.A.GetEnumName(), "A");
            Assert.Equal(TestEnum.B.GetEnumName(), "B");
            Assert.Equal(TestEnum.C.GetEnumName(), "C");
            Assert.Equal(TestEnum.D.GetEnumName(), "D");
        }

        [Fact(DisplayName = "EnumExtension.GetCustomAttributes")]
        public void GetCustomAttributes() {
            Assert.Equal(TestEnum.A.GetCustomAttributes<TestEnum, TestAttr1>().Count(), 2);
        }

        [Fact(DisplayName = "EnumExtension.GetCustomAttribute")]
        public void GetCustomAttribute() {
            Assert.NotNull(TestEnum.B.GetCustomAttribute<TestEnum, TestAttr2>());
        }
    }
}
