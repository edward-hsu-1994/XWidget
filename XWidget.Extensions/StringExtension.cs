using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace System {
    /// <summary>
    /// 針對<see cref="string"/>的擴充方法
    /// </summary>
    public static partial class StringExtension {
        /// <summary>
        /// 安全的從目前實例擷取子字串。子字串會在指定的字元開始並繼續到字串結尾
        /// </summary>
        /// <param name="obj">字串實例</param>
        /// <param name="startIndex">起始索引</param>
        /// <param name="length">擷取子字串最長長度</param>
        /// <returns>子字串</returns>
        public static string SafeSubstring(this string obj, int startIndex, int? length = null) {
            if (!length.HasValue) length = obj.Length;
            if (obj.Length <= startIndex) return string.Empty;
            if (startIndex < 0) startIndex = 0;
            if (length < 0) length = 0;
            string result = obj.Substring(startIndex);
            length = Math.Min(result.Length, length.Value);
            return result.Substring(0, length.Value);
        }

        /// <summary>
        /// 檢查字串是否符合表示式
        /// </summary>
        /// <param name="obj">字串實例</param>
        /// <param name="pattern">正規表示式</param>
        /// <returns>是否符合表示式</returns>
        public static bool IsMatch(this string obj, string pattern) {
            Regex regex = new Regex(pattern);
            return regex.IsMatch(obj);
        }

        /// <summary>
        /// 使用正規表示式切割字串
        /// </summary>
        /// <param name="obj">字串實例</param>
        /// <param name="pattern">正規表示式</param>
        /// <returns>切割結果</returns>
        public static string[] SplitByRegex(this string obj, string pattern) {
            return new Regex(pattern).Split(obj);
        }

        /// <summary>
        /// 使用正規表示式切割字串
        /// </summary>
        /// <param name="obj">字串實例</param>
        /// <param name="pattern">正規表示式</param>
        /// <param name="count">數量</param>
        /// <returns>切割結果</returns>
        public static string[] SplitByRegex(this string obj, string pattern, int count) {
            return new Regex(pattern).Split(obj, count);
        }

        /// <summary>
        /// 使用正規表示式切割字串
        /// </summary>
        /// <param name="obj">字串實例</param>
        /// <param name="pattern">正規表示式</param>
        /// <param name="count">數量</param>
        /// <param name="startat">起始索引</param>
        /// <returns>切割結果</returns>
        public static string[] SplitByRegex(this string obj, string pattern, int count, int startat) {
            return new Regex(pattern).Split(obj, count, startat);
        }

        /// <summary>
        /// 取得指定字串間的字串
        /// </summary>
        /// <param name="obj">字串實例</param>
        /// <param name="start">起始字串</param>
        /// <param name="end">結束字串</param>
        /// <returns>字串間的字串</returns>
        public static string InnerString(this string obj, string start, string end) {
            string result = obj.SafeSubstring(obj.IndexOf(start) + start.Length);
            return result.SafeSubstring(0, result.IndexOf(end));
        }

        /// <summary>
        /// 在指定的索引位置切割字串
        /// </summary>
        /// <param name="obj">字串實例</param>
        /// <param name="sliceIndexes">切割索引</param>
        /// <returns>切割後的字串陣列</returns>
        public static string[] Slice(this string obj, params int[] sliceIndexes) {
            List<string> result = new List<string>();

            string temp = obj;

            int preIndex = 0;
            foreach (var index in sliceIndexes) {
                result.Add(temp.SafeSubstring(0, index - preIndex));
                temp = temp.SafeSubstring(index - preIndex);

                preIndex = index;
            }

            result.Add(temp);

            return result.ToArray();
        }
    }
}
