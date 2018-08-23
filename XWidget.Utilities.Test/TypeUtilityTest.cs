using MathUtility.Utilities.Test.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace XWidget.Utilities.Test {
    public class TypeUtilityTest {
        [Fact(DisplayName = "TypeUtility.GetAllBaseTypes")]
        public void GetAllBaseTypes() {
            var types = TypeUtility.GetNamespaceTypes("XWidget.Utilities.Test.Models");
            var types2 = new Type[] {
                typeof(OSType),
                typeof(TestClass1),
                typeof(TestClass2),
                typeof(TestEnumAttribute),
                typeof(TestEnum2Attribute)
            };

            Assert.Empty(types.Except(types2));
        }

        [Fact(DisplayName = "TypeUtility.IntParse")]
        public void IntParse() {
            Assert.Equal(255, TypeUtility.Parse<int>("255"));
        }

        [Fact(DisplayName = "TypeUtility.TryParse")]
        public void TryParse() {
            if (TypeUtility.TryParse<int>("255", out int INT)) {
                Assert.Equal(255, INT);
            }
            if (TypeUtility.TryParse<bool>("true", out bool BOOL)) {
                Assert.True(BOOL);
            }
        }
    }
}
