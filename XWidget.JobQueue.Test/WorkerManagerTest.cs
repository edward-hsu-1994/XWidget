using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
            IWorker worker = new WorkerManager(2);

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

        [Fact(DisplayName = "XWidget.JobQueue.WorkerManagerTest3")]
        public void Test3() {
            IWorker worker = new WorkerManager(1);

            int value = 0;
            worker.Add(new Job<int>(job => {
                Thread.Sleep(1000);

                value += 2;

                return value;
            }));

            Job<int> job2;
            worker.Add((job2 = new Job<int>(job => {
                throw new NotImplementedException();
            }))
                .Subscribe(
                    (int x) => { },
                    (Exception e) => {
                        value += 8;
                    },
                () => { }));

            value += -10;

            worker.Remove(job2);

            worker.WaitForIdle();

            worker.Reboot();

            Assert.Equal(-8, value);
        }


        [Fact(DisplayName = "XWidget.JobQueue.WorkerManagerTest4")]
        public void Test4() {
            WorkerManager worker = new WorkerManager(1);


            var job1 = new Job<int>(j => {
                Thread.Sleep(500);
                return 1;
            });
            var job2 = new Job<int>(j => {
                Thread.Sleep(500);
                return 2;
            });
            worker.Add(job1);
            worker.Add(job2);

            Assert.Equal(2, worker.JobQueue.Count);

            Assert.True(worker.JobQueue.First().Id == job1.Id);

            worker.Remove(job2);
            worker.WaitForIdle();

            Assert.False(worker.IsDead);

            Assert.True(worker.IsIdle);

            Assert.Empty(worker.JobQueue);
        }
    }
}
