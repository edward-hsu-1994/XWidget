using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XWidget.Extensions;
using Xunit;

namespace XWidget.Extensions.Test {
    public class StringExtensionTest {
        [Theory(DisplayName = "StringExtension.SafeSubstring")]
        [InlineData("0123456789", 0, 5, "01234")]
        [InlineData("0123456789", 3, 5, "34567")]
        [InlineData("0123456789", 7, 5, "789")]
        [InlineData("0123456789", -1, 5, "01234")]
        public void SafeSubstring(string text, int index, int length, string result) {
            Assert.Equal(text.SafeSubstring(index, length), result);
        }

        [Theory(DisplayName = "StringExtension.IsMatch")]
        [InlineData("0123456789", @"\d+", true)]
        [InlineData("A123456789", @"\w\d{9}", true)]
        [InlineData("dddddddd", @"a+", false)]
        public void IsMatch(string text, string pattern, bool isMatch) {
            Assert.Equal(text.IsMatch(pattern), isMatch);
        }

        public static IEnumerable<object[]> SplitByRegexData {
            get {
                return new object[][]{
                    new object[]{"1,2,3,4 ,5,6, 7,8, 9", @"\W*,\W*", "1,2,3,4,5,6,7,8,9".Split(',') },
                    new object[]{"1   2 3  4    5 6  7 8  9", @"\W+", "1,2,3,4,5,6,7,8,9".Split(',') }
                }.ToList();
            }
        }

        [Theory(DisplayName = "StringExtension.SplitByRegex")]
        [MemberData(nameof(SplitByRegexData))]
        public void SplitByRegex(string text, string pattern, string[] result) {
            Assert.Equal(text.SplitByRegex(pattern), result);
        }

        public static IEnumerable<object[]> SplitByRegexCountData {
            get {
                return new object[][]{
                    new object[]{"1,2,3,4 ,5,6, 7,8, 9", @"\W*,\W*", 2, new string[]{
                        "1", "2,3,4 ,5,6, 7,8, 9"
                    } },
                    new object[]{"1   2 3  4    5 6  7 8  9", @"\W+", 3, new string[] {
                        "1", "2", "3  4    5 6  7 8  9"
                    } }
                }.ToList();
            }
        }

        [Theory(DisplayName = "StringExtension.SplitByRegex")]
        [MemberData(nameof(SplitByRegexCountData))]
        public void SplitByRegexCount(string text, string pattern, int count, string[] result) {
            Assert.Equal(text.SplitByRegex(pattern, count), result);
        }

        [Theory(DisplayName = "StringExtension.InnerString")]
        [InlineData("0123456789", "0", "9", "12345678")]
        [InlineData("0123456789", "5", "0", "")]
        [InlineData("0123456789", "9", "6", "")]
        [InlineData("{{{}}}", "{", "}", "{{")]

        public void InnerString(string text, string start, string end, string result) {
            Assert.Equal(text.InnerString(start, end), result);
        }

        [Theory(DisplayName = "StringExtension.Slice")]
        [InlineData("0123456789", new int[] { 1, 2 }, new string[] { "0", "1", "23456789" })]
        [InlineData("0123456789", new int[] { 4, 8, 9 }, new string[] { "01234", "567", "8", "9" })]
        [InlineData("0123456789", new int[] { 5 }, new string[] { "01234", "56789" })]

        public void Slice(string text, int[] indexes, string[] result) {
            Assert.Equal(text.Slice(indexes), result);
        }
    }
}
