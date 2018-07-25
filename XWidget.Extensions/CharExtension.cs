using System;
using System.Collections.Generic;
using System.Text;

namespace System {
    /// <summary>
    /// 針對<see cref="char"/>的擴充方法
    /// </summary>
    public static class CharExtension {
        /// <summary>
        /// 檢查字元是否在指定區間內
        /// </summary>
        /// <param name="char">字元</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns>是否在區間內</returns>
        private static bool Between(this char @char, int min, int max) {
            return @char >= min && @char <= max;
        }

        /// <summary>
        /// Unicode CJK範圍
        /// </summary>
        private static Func<char, string>[] LangRanges = new Func<char, string>[] {
            (c)=> {//Unicode CJK範圍
                if(c.Between(0x2E80, 0x2EFF) ||
                   c.Between(0x3000, 0x303F) ||
                   c.Between(0x3200, 0x32FF) ||
                   c.Between(0x3300, 0x33FF) ||
                   c.Between(0x3400, 0x4DBF) ||
                   c.Between(0x4E00, 0x9FFF) ||
                   c.Between(0xF900, 0xFAFF) ||
                   c.Between(0xFE30, 0xFE4F) ||
                   c.Between(0x20000, 0x2A6DF) ||
                   c.Between(0x2F800, 0x2FA1F)) {
                    return "CJK";
                }
                return null;
            },
            (c)=> {
                return "other";
            }
        };

        /// <summary>
        /// 取得指定字元語系
        /// </summary>
        /// <param name="c">字元</param>
        /// <returns>語系名稱</returns>
        public static string GetLangType(this char c) {
            foreach (var func in LangRanges) {
                var result = func(c);
                if (result != null) return result;
            }
            return null;
        }
    }
}
