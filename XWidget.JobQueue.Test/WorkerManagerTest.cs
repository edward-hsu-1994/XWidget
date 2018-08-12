using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace XWidget.JobQueue.Test {
    public class WorkerManagerTest {
        [Fact(DisplayName = "XWidget.JobQueue.WorkerManagerTest1")]
        public void Test1() {
            IWorker worker = new WorkerManager(2);

            int value = 5;
            worker.Add(new Job<int>(job => {
                value += 2;

                return value;
            }));

            value += 2;

            worker.WaitForIdle();

            Assert.Equal(9, value);
        }

        [Fact(DisplayName = "XWidget.JobQueue.WorkerManagerTest2")]
        public void Test2() {
            IWorker worker = new WorkerManager();

            int value = 0;
            worker.Add(new Job<int>(job => {
                value += 2;

                return value;
            }));

            worker.Add(new Job<int>(job => {
                throw new NotImplementedException();
            }).Subscribe(
                (int x) => { },
                (Exception e) => {
                    value += 8;
                },
                () => { }));

            value += -10;

            worker.WaitForIdle();

            Assert.Equal(0, value);
        }
    }
}
