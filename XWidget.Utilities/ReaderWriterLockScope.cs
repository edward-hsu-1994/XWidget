using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace XWidget.Utilities {
    /// <summary>
    /// 讀寫鎖區塊模式
    /// </summary>
    public enum LockMode {
        Write,
        Read,
        UpgradeableRead
    }

    /// <summary>
    /// 讀寫鎖區塊
    /// </summary>
    public class ReaderWriterLockScope : IDisposable {
        LockMode mode;
        ReaderWriterLockSlim locker;
        public ReaderWriterLockScope(ReaderWriterLockSlim locker, LockMode mode) {
            this.locker = locker;
            this.mode = mode;
            switch (mode) {
                case LockMode.Read:
                    locker.EnterReadLock();
                    break;
                case LockMode.Write:
                    locker.EnterWriteLock();
                    break;
                case LockMode.UpgradeableRead:
                    locker.EnterUpgradeableReadLock();
                    break;
            }
        }

        public void Dispose() {
            switch (mode) {
                case LockMode.Read:
                    locker.ExitReadLock();
                    break;
                case LockMode.Write:
                    locker.ExitWriteLock();
                    break;
                case LockMode.UpgradeableRead:
                    locker.ExitUpgradeableReadLock();
                    break;
            }
        }
    }
}
