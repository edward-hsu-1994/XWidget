using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace System {
    /// <summary>
    /// 針對<see cref="byte"/>的擴充方法
    /// </summary>
    public static class ByteExtension {
        /// <summary>
        /// 將<see cref="byte[]"/>轉換為16進位表示
        /// </summary>
        /// <param name="binary">Binary Data</param>
        /// <returns>16進位表示</returns>
        public static string ToHex(this byte[] binary, bool upper = false) {
            return string.Join("", binary.Select(x => x.ToString(upper ? "x2" : "X2")));
        }

        /// <summary>
        /// <see cref="byte[]"/>轉換為<see cref="Stream"/>
        /// </summary>
        /// <param name="binary">Binary Data</param>
        /// <returns><see cref="Stream"/>串流實例</returns>
        public static Stream ToStream(this byte[] binary) {
            Stream stream = new MemoryStream(binary);
            return stream;
        }

        /// <summary>
        /// 將Binary Data轉換為目標類別實例
        /// </summary>
        /// <typeparam name="T">目標類別</typeparam>
        /// <param name="bytes">Binary Data</param>
        /// <returns>目標類別實例</returns>
        public static T Deserialize<T>(this byte[] bytes) {
            BinaryFormatter sf = new BinaryFormatter();
            return (T)sf.Deserialize(bytes.ToStream());
        }
    }
}
