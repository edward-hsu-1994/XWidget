using System;
using System.Collections.Generic;
using System.Text;

namespace XWidget.Utilities {
    /// <summary>
    /// 有關重試操作相關常用方法
    /// </summary>
    public static class TryUtility {
        /// <summary>
        /// 重試操作
        /// </summary>
        /// <typeparam name="T">回應類型</typeparam>
        /// <param name="retryTime">重試次數</param>
        /// <param name="func">操作方法</param>
        /// <returns>操作結果</returns>
        public static T Retry<T>(int retryTime, Func<T> func) {
            Exception exception = null;
            for (int i = 0; i < retryTime; i++) {
                try {
                    return func();
                } catch (Exception e) {
                    exception = e;
                }
            }
            throw exception;
        }

        /// <summary>
        /// 重試操作
        /// </summary>
        /// <param name="retryTime">重試次數</param>
        /// <param name="action">操作方法</param>
        public static void Retry(int retryTime, Action action) {
            Retry(retryTime, () => {
                action();
                return 0;
            });
        }
    }
}
