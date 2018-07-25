using System;

namespace System.Threading.Tasks {
    /// <summary>
    /// 針對<see cref="Task"/>類別的擴充方法
    /// </summary>
    public static class TaskExtension {
        /// <summary>
        /// 將非同步方法轉換為同步並取得執行結果
        /// </summary>
        /// <typeparam name="T">非同步類型</typeparam>
        /// <param name="task">非同步程序實例</param>
        /// <returns>方法執行結果</returns>
        public static T ToSync<T>(this Task<T> task) {
            return task.GetAwaiter().GetResult();
        }

        /// <summary>
        /// 將非同步方法轉換為同步並取得執行結果
        /// </summary>
        /// <param name="task">非同步程序實例</param>
        /// <returns>方法執行結果</returns>
        public static void ToSync(this Task task) {
            task.GetAwaiter().GetResult();
        }
    }
}
