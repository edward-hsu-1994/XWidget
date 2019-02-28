using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace XWidget.Utilities {
    /// <summary>
    /// 讀寫鎖區塊擴充方法
    /// </summary>
    public static class ReaderWriterLockScopeExtension {
        /// <summary>
        /// 產生讀寫鎖區塊
        /// </summary>
        /// <param name="locker">讀寫鎖</param>
        /// <param name="mode">模式</param>
        /// <returns>讀寫鎖區塊</returns>
        public static ReaderWriterLockScope Scope(this ReaderWriterLockSlim locker, LockMode mode) {
            return new ReaderWriterLockScope(locker, mode);
        }
    }
}
