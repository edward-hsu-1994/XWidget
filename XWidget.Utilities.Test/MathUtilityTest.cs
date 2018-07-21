using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace XWidget.Utilities.Test {
    public class MathUtilityTest {
        [Theory(DisplayName = "MathUtility.LCM")]
        [InlineData(3, 5, 15)]
        [InlineData(3, 1, 3)]
        [InlineData(7, 5, 35)]

        public void LCM(int value1, int value2, int lcm) {
            Assert.Equal(MathUtility.LCM(value1, value2), lcm);
        }

        [Theory(DisplayName = "MathUtility.LCM(array)")]
        [InlineData(3, 5, 7, 105)]
        [InlineData(3, 1, 4, 12)]
        [InlineData(7, 5, 10, 70)]

        public void LCM_Array(int value1, int value2, int value3, int lcm) {
            Assert.Equal(MathUtility.LCM(new int[] { value1, value2, value3 }), lcm);
        }

        [Theory(DisplayName = "MathUtility.GCD")]
        [InlineData(5645, 485, 5)]
        [InlineData(68, 241, 1)]
        [InlineData(66, 33, 33)]

        public void GCD(int value1, int value2, int gcd) {
            Assert.Equal(MathUtility.GCD(value1, value2), gcd);
        }

        [Theory(DisplayName = "MathUtility.GCD(array)")]
        [InlineData(3, 5, 7, 1)]
        [InlineData(3, 6, 18, 3)]
        [InlineData(80, 60, 70, 10)]

        public void GCD_Array(int value1, int value2, int value3, int gcd) {
            Assert.Equal(MathUtility.GCD(new int[] { value1, value2, value3 }), gcd);
        }
    }
}
