using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace XWidget.Extensions.Test {
    public class ObjectExtensionTest {
        public class TestClass {
            private string _PrivateValue = "aabbcc";
            public string _PublicValue_0 = "";
        }

        public class TestClassChild : TestClass {
            public string _PublicValue_1 = "112233";
        }

        [Fact(DisplayName = "ObjectExtensionTest.GetPrivateFieldValue")]
        public void GetPrivateFieldValue() {
            Assert.Equal(
                new TestClass().GetPrivateFieldValue<string>("_PrivateValue"),
                "aabbcc"
                );
        }

        [Fact(DisplayName = "ObjectExtensionTest.ToChildType")]
        public void ToChildType() {
            var obj = new TestClass() {
                _PublicValue_0 = "__AA__!!"
            };
            var convertObj = obj.ToChildType<TestClass, TestClassChild>();

            Assert.Equal(obj._PublicValue_0, convertObj._PublicValue_0);
        }

        [Fact(DisplayName = "ObjectExtensionTest.Process")]
        public void Process() {
            TestClassChild GetObj() {
                return new TestClassChild() {
                    _PublicValue_0 = "a",
                    _PublicValue_1 = "b"
                };
            }

            var obj = GetObj()
                .Process(x => x._PublicValue_0 = "bbbb")
                .Process(x => x._PublicValue_1 = "cccc");

            Assert.Equal("bbbb", obj._PublicValue_0);
            Assert.Equal("cccc", obj._PublicValue_1);
        }
    }
}
