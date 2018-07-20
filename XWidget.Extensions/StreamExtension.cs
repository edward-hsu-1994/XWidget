using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace System {
    /// <summary>
    /// 針對<see cref="Stream"/>的擴充方法
    /// </summary>
    public static class StreamExtension {
        /// <summary>
        /// 串流轉換為<see cref="byte[]"/>
        /// </summary>
        /// <param name="stream">串流實例</param>
        /// <returns><see cref="byte[]"/>Binary Data</returns>
        public static byte[] ToBytes(this Stream stream) {
            if (stream.CanSeek) {

                byte[] bytes = new byte[stream.Length];

                stream.Seek(0, SeekOrigin.Begin);
                stream.Read(bytes, 0, bytes.Length);

                return bytes;
            } else {
                List<byte> result = new List<byte>();
                int i;
                while ((i = stream.ReadByte()) > -1) {
                    result.Add((byte)i);
                };
                return result.ToArray();
            }

        }
    }
}
