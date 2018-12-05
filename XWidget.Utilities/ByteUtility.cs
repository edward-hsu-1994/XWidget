using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XWidget.Utilities {
    /// <summary>
    /// 針對<see cref="byte"/>常用方法
    /// </summary>
    public static class ByteUtility {
        #region 二進制進位
        /// <summary>
        /// Kibibyte
        /// </summary>
        public const long KiB = 1024;

        /// <summary>
        /// Mebibyte
        /// </summary>
        public const long MiB = KiB * 1024;

        /// <summary>
        /// Gibibyte
        /// </summary>
        public const long GiB = MiB * 1024;

        /// <summary>
        /// Tebibyte
        /// </summary>
        public const long TiB = GiB * 1024;

        /// <summary>
        /// Pebibyte
        /// </summary>
        public const long PiB = TiB * 1024;

        /// <summary>
        /// Exbibyte
        /// </summary>
        public const long EiB = PiB * 1024;
        #endregion

        #region 十進制進位
        /// <summary>
        /// Kilobyte
        /// </summary>
        public const long KB = 1000;

        /// <summary>
        /// Megabyte
        /// </summary>
        public const long MB = KB * 1000;

        /// <summary>
        /// Gigabyte
        /// </summary>
        public const long GB = MB * 1000;

        /// <summary>
        /// Terabyte
        /// </summary>
        public const long TB = GB * 1000;

        /// <summary>
        /// Petabyte
        /// </summary>
        public const long PB = TB * 1000;

        /// <summary>
        /// Exabyte
        /// </summary>
        public const long EB = PB * 1000;
        #endregion

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
