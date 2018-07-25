using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using XWidget.Cryptography;

namespace System.IO {
    /// <summary>
    /// 針對<see cref="FileInfo"/>Cryptography的擴充方法
    /// </summary>
    public static class FileInfoExtension {
        /// <summary>
        /// 將<see cref="FileInfo"/>使用指定的雜湊演算法轉換為雜湊
        /// </summary>
        /// <typeparam name="Algorithm">雜湊演算法型別</typeparam>
        /// <param name="info">檔案資訊</param>
        /// <returns>雜湊Binary</returns>
        public static byte[] ToHash<Algorithm>(this FileInfo info) where Algorithm : HashAlgorithm {
            using (var stream = info.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                return HashHelper.ToHash<Algorithm>(stream);
            }
        }

        /// <summary>
        /// 將<see cref="FileInfo"/>使用指定的雜湊演算法轉換為雜湊後在轉換為16進位字串表示
        /// </summary>
        /// <typeparam name="Algorithm">雜湊演算法型別</typeparam>
        /// <param name="info">檔案資訊</param>
        /// <param name="upper">是否轉換為大寫</param>
        /// <returns>雜湊字串</returns>
        public static string ToHashString<Algorithm>(this FileInfo info, bool upper = true) where Algorithm : HashAlgorithm {
            using (var stream = info.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                return HashHelper.ToHashString<Algorithm>(stream, upper);
            }
        }
    }
}
