using System;
using System.Security.Cryptography;
using Xunit;
using XWidget.Utilities;

namespace XWidget.Cryptography.Test {
    public class StringExtensionTest {
        [Theory(DisplayName = "StringExtension.ToHashString")]
        [InlineData("1234", "81dc9bdb52d04dc20036dbd8313ed055")]
        [InlineData("abc", "900150983cd24fb0d6963f7d28e17f72")]
        [InlineData("dddd", "11ddbaf3386aea1f2974eee984542152")]
        public void ToHashString(string str, string result) {
            Assert.Equal(str.ToHashString<MD5>(true), result.ToUpper());
            Assert.Equal(str.ToHashString<MD5>(false), result.ToLower());
        }

        [Theory(DisplayName = "StringExtension.ToHashString")]
        [InlineData("1234", "81dc9bdb52d04dc20036dbd8313ed055")]
        [InlineData("abc", "900150983cd24fb0d6963f7d28e17f72")]
        [InlineData("dddd", "11ddbaf3386aea1f2974eee984542152")]
        public void ToHash(string str, string result) {
            Assert.Equal(str.ToHash<MD5>(), ByteUtility.FromHex(result));
            Assert.Equal(str.ToHash<MD5>(), ByteUtility.FromHex(result));
        }
    }
}
