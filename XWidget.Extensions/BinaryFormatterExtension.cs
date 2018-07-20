using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace System.Runtime.Serialization.Formatters.Binary {
    /// <summary>
    /// 針對<see cref="BinaryFormatter"/>的擴充方法
    /// </summary>
    public static class BinaryFormatterExtension {
        /// <summary>
        /// 將目標實例序列化為<see cref="byte[]"/>
        /// </summary>
        /// <param name="formatter"><see cref="BinaryFormatter"/>實例</param>
        /// <param name="obj">目標實例</param>
        /// <returns>目標實例序列化結果</returns>
        public static byte[] Serialize(this BinaryFormatter formatter, object obj) {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, obj);
            return ms.ToBytes();
        }

        /// <summary>
        /// 將Binary結果反序列化為目標類別實例
        /// </summary>
        /// <param name="formatter"><see cref="BinaryFormatter"/>實例</param>
        /// <param name="type">目標類別</param>
        /// <param name="binary">目標實例Binary結果</param>
        /// <returns>目標實例</returns>
        public static object Deserialize(this BinaryFormatter formatter, Type type, byte[] binary) {
            MemoryStream ms = new MemoryStream(binary);
            return Convert.ChangeType(formatter.Deserialize(ms), type);
        }

        /// <summary>
        /// 將Binary結果反序列化為目標類別實例
        /// </summary>
        /// <typeparam name="T">目標類別</typeparam>
        /// <param name="formatter"><see cref="BinaryFormatter"/>實例</param>
        /// <param name="binary">目標實例Binary結果</param>
        /// <returns>目標實例</returns>
        public static T Deserialize<T>(this BinaryFormatter formatter, byte[] binary) {
            return (T)Deserialize(formatter, typeof(T), binary);
        }
    }
}
