using System;
using Xunit;

namespace XWidget.JobQueue.Test {
    public class SingleWorkerTest {
        [Fact(DisplayName = "XWidget.JobQueue.SingleWorkerTest1")]
        public void Test1() {
            IWorker worker = new Worker();

            int value = 5;
            worker.Add(new Job<int>(job => {
                value += 2;

                return value;
            }));

            value += 2;

            worker.WaitForIdle();

            Assert.Equal(9, value);
        }

        [Fact(DisplayName = "XWidget.JobQueue.SingleWorkerTest2")]
        public void Test2() {
            IWorker worker = new Worker();

            int value = 1;
            worker.Add(new Job<int>(job => {
                value -= 2;

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

            value += 9;

            worker.WaitForIdle();

            Assert.Equal(16, value);
        }
    }
}
