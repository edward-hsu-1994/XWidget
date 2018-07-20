using System;
using System.Collections.Generic;
using System.Text;

namespace XWidget.Utilities {
    /// <summary>
    /// 數學常用方法
    /// </summary>
    public static class MathUtility {
        /// <summary>
        /// 求陣列數值最小公倍數
        /// </summary>
        public static int LCM(this int[] values) {
            int result = values[0];
            foreach (int value in values) {
                result = LCM(result, value);
            }
            return result;
        }

        /// <summary>
        /// 求兩數值之最小公倍數
        /// </summary>
        /// <param name="m">數值一</param>
        /// <param name="n">數值二</param>
        /// <returns></returns>
        public static int LCM(int m, int n) {
            return m * n / GCD(m, n);
        }

        /// <summary>
        /// 求陣列數值之最大公因數
        /// </summary>
        /// <param name="obj">目前實例</param>
        /// <returns>最大公因數</returns>
        public static int GCD(this int[] obj) {
            int result = obj[0];
            foreach (int value in obj) {
                result = GCD(result, value);
                if (result == 1) return 1;
            }
            return result;
        }

        /// <summary>
        /// 求兩數值之最大公因數
        /// </summary>
        /// <param name="m">數值一</param>
        /// <param name="n">數值二</param>
        /// <returns>最大公因數</returns>
        public static int GCD(int m, int n) {
            if (n == 0) {
                return m;
            } else {
                return GCD(n, m % n);
            }
        }
    }
}
