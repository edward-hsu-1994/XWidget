using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XWidget.Utilities {
    /// <summary>
    /// 針對<see cref="byte"/>常用方法
    /// </summary>
    public static class ByteUtility {
        /// <summary>
        /// 將<see cref="byte[]"/>轉換為16進位表示
        /// </summary>
        /// <param name="binary">Binary Data</param>
        /// <returns>16進位表示</returns>
        public static string ToHex(this byte[] binary, bool upper = false) {
            return string.Join("", binary.Select(x => x.ToString(upper ? "x2" : "X2")));
        }

        /// <summary>
        /// 將16進位表示轉換為<see cref="byte[]"/>
        /// </summary>
        /// <param name="str">16進位表示</param>
        /// <returns>Binary Data</returns>
        public static byte[] FromHex(this string str) {
            return StringUtility.Split(str, 2)
                .Select(x => byte.Parse(x, System.Globalization.NumberStyles.HexNumber))
                .ToArray();
        }
    }
}
