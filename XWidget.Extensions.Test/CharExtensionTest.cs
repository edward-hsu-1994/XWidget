using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace XWidget.Extensions.Test {
    public class CharExtensionTest {
        [Fact(DisplayName = "CharExtension.GetLangType")]
        public void GetLangType() {
            Assert.Equal('爽'.GetLangType(), "CJK");
            Assert.Equal('A'.GetLangType(), "other");
        }
    }
}
