using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace System {
    /// <summary>
    /// 針對<see cref="Random"/>的擴充方法
    /// </summary>
    public static class RandomExtension {
        /// <summary>
        /// 隨機取指定數值內的<see cref="double"/>值
        /// </summary>
        /// <param name="rand"><see cref="Random"/>實例</param>
        /// <param name="min">下限</param>
        /// <param name="max">獨佔上限</param>
        /// <returns>亂數結果</returns>
        public static double NextDouble(this Random rand, double min, double max) {
            return rand.NextDouble(max - min) + min;
        }

        /// <summary>
        /// 隨機取得0到指定數值內的<see cref="double"/>值值
        /// </summary>
        /// <param name="rand"><see cref="Random"/>實例</param>
        /// <param name="max">獨佔上限</param>
        /// <returns>亂數結果</returns>
        public static double NextDouble(this Random rand, double max) {
            return rand.NextDouble() * max;
        }

        /// <summary>
        /// 隨機取得<see cref="bool"/>值值
        /// </summary>
        /// <param name="rand"><see cref="Random"/>實例</param>
        /// <returns>亂數結果</returns>
        public static bool NextBool(this Random rand) {
            return rand.NextDouble() > 0.5;
        }

        /// <summary>
        /// 隨機自列舉型別中取得值
        /// </summary>
        /// <param name="rand"><see cref="Random"/>實例</param>
        /// <param name="type">目標型別</param>
        /// <returns>亂數結果</returns>
        public static object NextEnum(this Random rand, Type type) {
            Array values = type.GetTypeInfo().GetEnumValues();
            int index = rand.Next(values.Length);
            return values.GetValue(index);
        }

        /// <summary>
        /// 隨機自列舉型別中取得值
        /// </summary>
        /// <param name="rand"><see cref="Random"/>實例</param>
        /// <returns>亂數結果</returns>
        public static T NextEnum<T>(this Random rand) {
            return (T)NextEnum(rand, typeof(T));
        }

        /// <summary>
        /// 隨機自字串陣列中取得值
        /// </summary>
        /// <param name="rand"><see cref="Random"/>實例</param>
        /// <param name="data">目標字串</param>
        /// <returns>亂數結果</returns>
        public static string NextString(this Random rand, params string[] data) {
            int index = rand.Next(data.Length);
            return data[index];
        }

        /// <summary>
        /// 隨機自陣列中取得值
        /// </summary>
        /// <param name="rand"><see cref="Random"/>實例</param>
        /// <param name="data">目標字串</param>
        /// <returns>亂數結果</returns>
        public static T NextArrayElement<T>(this Random rand, params T[] data) {
            int index = rand.Next(data.Length);
            return data[index];
        }
    }
}
