using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XWidget.Utilities {
    /// <summary>
    /// 針對<see cref="String"/>常用方法
    /// </summary>
    public static class StringUtility {
        /// <summary>
        /// 字串中不同語系文字中插入空白字元
        /// </summary>
        /// <param name="str">字串實例</param>
        /// <returns>自動加入空白後的字串</returns>
        public static string Spacing(string str) {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < str.Length - 1; i++) {
                builder.Append(str[i]);
                if (str[i] == ' ' || str[i + 1] == ' ') continue;
                if (str[i].GetLangType() != str[i + 1].GetLangType()) {
                    builder.Append(' ');
                }
            }

            builder.Append(str.Last());
            return builder.ToString();
        }

        /// <summary>
        /// 將字串根據指定區塊大小切割
        /// </summary>
        /// <param name="str">字串實例</param>
        /// <param name="chunkSize">區塊大小</param>
        /// <returns>切割後的字串</returns>
        public static string[] Split(string str, int chunkSize) {
            return Enumerable.Range(0, str.Length / chunkSize)
                .Select(x => str.Substring(x * chunkSize, chunkSize))
                .ToArray();
        }
    }
}
