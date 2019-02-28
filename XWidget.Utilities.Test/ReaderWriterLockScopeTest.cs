using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace XWidget.Utilities.Test {
    public class ReaderWriterLockScopeTest {
        [Fact]
        public void WriteLockScopeTest() {
            ReaderWriterLockSlim locker = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

            int i = 100;

            Parallel.For(0, 100, x => {
                using (locker.Scope(LockMode.Write)) {
                    i--;
                }
            });

            Assert.Equal(0, i);
        }

        [Fact]
        public void ReadLockScopeTest() {
            ReaderWriterLockSlim locker = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

            int i = 100;
            int j = 0;
            Parallel.For(0, 100, x => {
                int value = 0;
                using (locker.Scope(LockMode.Read)) {
                    value = i;
                    i--;
                }
                using (locker.Scope(LockMode.Write)) {
                    j += value;
                }
            });

            Assert.Equal(5050, j);
        }

        [Fact]
        public void ReadWriteLockScopeTest() {
            ReaderWriterLockSlim locker = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

            int i = 100;
            int j = 0;
            Parallel.For(0, 100, x => {
                int value = 0;
                using (locker.Scope(LockMode.UpgradeableRead)) {
                    value = i;
                    i--;
                    using (locker.Scope(LockMode.Write)) {
                        j += value;
                    }
                }
            });

            Assert.Equal(5050, j);
        }
    }
}
