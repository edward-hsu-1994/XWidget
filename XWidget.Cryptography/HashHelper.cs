using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace XWidget.Cryptography {
    /// <summary>
    /// 雜湊計算幫助類別
    /// </summary>
    public static class HashHelper {
        /// <summary>
        /// 將字串使用指定的雜湊演算法轉換為雜湊
        /// </summary>
        /// <typeparam name="Algorithm">雜湊演算法型別</typeparam>
        /// <param name="str">值</param>
        /// <returns>雜湊Binary</returns>
        public static byte[] ToHash<Algorithm>(string str) where Algorithm : HashAlgorithm {
            using (var hash = typeof(Algorithm)
                .GetMethod("Create", new Type[] { })
                .Invoke(null, null) as HashAlgorithm) {
                return hash.ComputeHash(Encoding.UTF8.GetBytes(str));
            }
        }

        /// <summary>
        /// 將字串使用指定的雜湊演算法轉換為雜湊後在轉換為16進位字串表示
        /// </summary>
        /// <typeparam name="Algorithm">雜湊演算法型別</typeparam>
        /// <param name="str">值</param>
        /// <param name="upper">是否轉換為大寫</param>
        /// <returns>雜湊字串</returns>
        public static string ToHashString<Algorithm>(string str, bool upper = true) where Algorithm : HashAlgorithm {
            return string.Join("", ToHash<Algorithm>(str).Select(x => x.ToString(upper ? "X2" : "x2")));
        }

        /// <summary>
        /// 將串流使用指定的雜湊演算法轉換為雜湊
        /// </summary>
        /// <typeparam name="Algorithm">雜湊演算法型別</typeparam>
        /// <param name="stream">串流實例</param>
        /// <returns>雜湊Binary</returns>
        public static byte[] ToHash<Algorithm>(Stream stream) where Algorithm : HashAlgorithm {
            using (var hash = typeof(Algorithm).GetMethod("Create", new Type[] { }).Invoke(null, null) as HashAlgorithm) {
                return hash.ComputeHash(stream);
            }
        }

        /// <summary>
        /// 將串流使用指定的雜湊演算法轉換為雜湊後在轉換為16進位字串表示
        /// </summary>
        /// <typeparam name="Algorithm">雜湊演算法型別</typeparam>
        /// <param name="stream">串流實例</param>
        /// <param name="upper">是否轉換為大寫</param>
        /// <returns>雜湊字串</returns>
        public static string ToHashString<Algorithm>(Stream stream, bool upper = true) where Algorithm : HashAlgorithm {
            return string.Join("", ToHash<Algorithm>(stream).Select(x => x.ToString(upper ? "X2" : "x2")));
        }
    }
}
