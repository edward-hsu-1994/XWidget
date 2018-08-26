using XWidget.Extensions.Test.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Xunit;

namespace XWidget.Extensions.Test {
    public class TypeExtensionTest {
        [Theory(DisplayName = "TypeExtension.GetAllBaseTypes")]
        [InlineData(typeof(TestClass1), "Object,TestClass1")]
        [InlineData(typeof(TestClass2), "Object,TestClass1,TestClass2")]
        public void GetAllBaseTypes(Type type, string typeName) {
            Assert.Equal(type.GetAllBaseTypes().Select(x => x.Name), typeName.Split(','));
        }

        [Theory(DisplayName = "TypeExtension.GetAllBaseTypeInfos")]
        [InlineData(typeof(TestClass1), "Object,TestClass1")]
        [InlineData(typeof(TestClass2), "Object,TestClass1,TestClass2")]
        public void GetAllBaseTypeInfos(Type type, string typeName) {
            Assert.Equal(type.GetTypeInfo().GetAllBaseTypeInfos().Select(x => x.Name), typeName.Split(','));
        }

        public static IEnumerable<object[]> IsAnonymousTypeData {
            get {
                return new object[][]{
                    new object[]{ typeof(TestClass1), false },
                    new object[]{ typeof(TestClass2), false },
                    new object[]{ typeof(string), false },
                    new object[]{ new { }.GetType(), true }
                }.ToList();
            }
        }

        [Theory(DisplayName = "TypeExtension.IsAnonymousType")]
        [MemberData(nameof(IsAnonymousTypeData))]
        public void IsAnonymousType(Type type, bool isAnonymousType) {
            Assert.Equal(type.IsAnonymousType(), isAnonymousType);
        }


        public static IEnumerable<object[]> GetDefaultData {
            get {
                return new object[][]{
                    new object[]{ new TestClass1(), default(TestClass1)},
                    new object[]{ new TestClass2(),  default(TestClass2) },
                    new object[]{ string.Empty,  default(string) },
                    new object[]{ Guid.NewGuid(), default(Guid) }
                }.ToList();
            }
        }

        [Theory(DisplayName = "TypeExtension.GetDefault<T>")]
        [MemberData(nameof(GetDefaultData))]
        public void GetDefault_T(object typeInstance, object defaultValue) {
            Assert.Equal(typeInstance.GetDefault(), defaultValue);
        }
    }
}
