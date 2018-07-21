using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using Xunit;

namespace XWidget.Reflection.Test {
    public class ExpandoObjectExtensionTest {
        [Fact(DisplayName = "ExpandoObjectExtension.CreateAnonymousType")]
        public void CreateAnonymousType() {
            var obj = new ExpandoObject();
            obj.TryAdd("Id", "1");

            var type = obj.CreateAnonymousType();

            Assert.NotNull(type.GetMember("Id"));
        }
    }
}
