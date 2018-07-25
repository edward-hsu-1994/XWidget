using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace XWidget.Extensions.Test {
    public class ArrayExtensionTest {
        public static IEnumerable<object[]> GetDefaultData {
            get {
                return new object[][]{
                    new object[]{ 1, new object[] { 1 } },
                    new object[]{ null, new object[] { null } },
                    new object[]{ new object[] { true }, new object[] { new object[] { true } } },
                }.ToList();
            }
        }

        [Theory(DisplayName = "ArrayExtension.BoxingToArray<T>")]
        [MemberData(nameof(GetDefaultData))]
        public void BoxingToArray_T(object elementInstance, object[] array) {
            Assert.Equal(elementInstance.BoxingToArray(), array);
        }

        public static IEnumerable<object[]> GetLengthsData {
            get {
                return new object[][]{
                    new object[]{ new object[,,] {
                        { { 1,2,3,4},{ 5,6,7,8} }
                    } , new int[]{ 1,2 ,4 } }
                }.ToList();
            }
        }

        [Theory(DisplayName = "ArrayExtension.GetLengths")]
        [MemberData(nameof(GetLengthsData))]
        public void GetLengths(Array array, int[] indexes) {
            Assert.Equal(array.GetLengths(), indexes);
        }

        public static IEnumerable<object[]> GetAllIndexesData {
            get {
                return new object[][]{
                    new object[]{ new object[,,] {
                        { { 1,2,3,4},{ 5,6,7,8} }
                    } , new int[][]{
                        new int[]{ 0,0,0 },
                        new int[]{ 0,0,1 },
                        new int[]{ 0,0,2 },
                        new int[]{ 0,0,3 },
                        new int[]{ 0,1,0 },
                        new int[]{ 0,1,1 },
                        new int[]{ 0,1,2 },
                        new int[]{ 0,1,3 },
                    }}
                }.ToList();
            }
        }

        [Theory(DisplayName = "ArrayExtension.GetAllIndexes")]
        [MemberData(nameof(GetAllIndexesData))]
        public void GetAllIndexes(Array array, int[][] indexes) {
            Assert.Equal(array.GetAllIndexes(), indexes);
        }

        public static IEnumerable<object[]> FullData {
            get {
                return new object[][]{
                    new object[]{ new object[,,] {
                        { { 1,2,3,4},{ 5,6,7,8} }
                    } , new object[,,] {
                        { { -1,-1,-1,-1},{ -1,-1,-1,-1} }
                    } , -1}
                }.ToList();
            }
        }

        [Theory(DisplayName = "ArrayExtension.SetValue")]
        [MemberData(nameof(FullData))]
        public void Full(Array array, Array result, object value) {
            array.Full(value);
            Assert.Equal(array, result);
        }
    }
}
