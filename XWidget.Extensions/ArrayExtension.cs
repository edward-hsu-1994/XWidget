using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System {
    /// <summary>
    /// 針對<see cref="Array"/>的擴充方法
    /// </summary>
    public static class ArrayExtension {
        /// <summary>
        /// 將目前實例包裝為目前實體類別陣列
        /// </summary>
        /// <typeparam name="T">目前實例類別</typeparam>
        /// <param name="obj">陣列實例</param>
        /// <returns>目前實體類別陣列包裝結果</returns>
        public static T[] BoxingToArray<T>(this T obj) {
            return new T[] { obj };
        }

        /// <summary>
        /// 取得代表<see cref="System.Array"/>所有維度之元素數目的 32 位元整數
        /// </summary>
        /// <param name="obj">陣列實例</param>
        /// <returns>所有維度之元素數目的 32 位元整數</returns>
        public static int[] GetLengths(this Array obj) {
            return Enumerable.Range(0, obj.Rank)
                .Select(x => obj.GetLength(x)).ToArray();
        }

        /// <summary>
        /// 取得代表<see cref="System.Array"/>所有元素的索引
        /// </summary>
        /// <param name="obj">陣列實例</param>
        /// <returns>32 位元的整數陣列的物件清單，代表所有元素的索引</returns>
        public static int[][] GetAllIndexes(this Array obj) {
            List<int> Indexes = obj.GetLengths().ToList();

            List<List<int>> C(List<int> input) {
                List<List<int>> result = new List<List<int>>();
                //if (input.Count == 0) return result;
                if (input.Count == 1) return Enumerable.Range(0, input.First()).Select(x => new List<int>(new int[] { x })).ToList();

                for (int i = 0; i < input.First(); i++) {
                    var r = C(input.Skip(1).ToList()).Select(x => {
                        x.Insert(0, i);
                        return x;
                    });
                    result.AddRange(r);
                }
                return result;
            };

            return C(Indexes).Select(x => x.ToArray()).ToArray();
        }

        /// <summary>
        /// 將輸入陣列填滿指定的值
        /// </summary>
        /// <param name="obj">陣列實例</param>
        /// <param name="value">指定數值</param>
        public static void Full(this Array obj, object value) {
            foreach (var index in obj.GetAllIndexes()) {
                obj.SetValue(value, index);
            }
        }
    }
}
