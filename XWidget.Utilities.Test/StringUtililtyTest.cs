using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace XWidget.Utilities.Test {
    public class StringUtililtyTest {
        [Theory(DisplayName = "StringUtililty.Spacing")]
        [InlineData("0123456789", "0123456789")]
        [InlineData("18歲", "18 歲")]
        [InlineData("第1名", "第 1 名")]
        [InlineData("總長度共24km長", "總長度共 24km 長")]

        public void Spacing(string text, string result) {
            Assert.Equal(StringUtility.Spacing(text), result);
        }
    }
}
