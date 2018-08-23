using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Collections.Generic {
    /// <summary>
    /// 針對<see cref="IEnumerable"/>的擴充方法
    /// </summary>
    public static class IEnumerableExtension {
        /// <summary>
        /// 切割列舉項目為指定長度
        /// </summary>
        /// <typeparam name="T">列舉元素類型</typeparam>
        /// <param name="obj">列舉實例</param>
        /// <param name="chunkSize">區段長度</param>
        /// <returns>切割後的列舉項目</returns>
        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> obj, int chunkSize) {
            var result = new List<List<T>>();

            for (int i = 0; i < obj.Count(); i += chunkSize) {
                result.Add(new List<T>(obj.Skip(i).Take(chunkSize)));
            }

            return result;
        }
    }
}
