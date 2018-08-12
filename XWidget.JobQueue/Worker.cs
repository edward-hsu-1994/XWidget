using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XWidget.JobQueue {
    /// <summary>
    /// 工作者
    /// </summary>
    public class Worker : IWorker {
        private List<IJob> jobQueue { get; } = new List<IJob>();

        private Task loop { get; set; }

        /// <summary>
        /// 工作列隊
        /// </summary>
        public IReadOnlyList<IJob> JobQueue => jobQueue.AsReadOnly();

        /// <summary>
        /// 是否閒置中
        /// </summary>
        public bool IsIdle {
            get {
                return loop == null || loop.IsCompleted || loop.IsFaulted || loop.IsCanceled;
            }
        }

        /// <summary>
        /// 主要工作執行緒中斷且還有剩餘工作
        /// </summary>
        public bool IsDead {
            get {
                return IsIdle && JobQueue.Count > 0;
            }
        }

        /// <summary>
        /// 新增工作
        /// </summary>
        /// <param name="job">工作</param>
        public void Add(IJob job) {
            lock (this) {
                jobQueue.Add(job);
            }

            Reboot();
        }

        /// <summary>
        /// 取消工作
        /// </summary>
        /// <param name="job">工作</param>
        public void Remove(IJob job) {
            Remove(job.Id);
        }

        /// <summary>
        /// 取消工作
        /// </summary>
        /// <param name="jobId">工作唯一識別號</param>
        public void Remove(Guid jobId) {
            lock (this) {
                var job = this.jobQueue.SingleOrDefault(x => x.Id == jobId);

                if (job == null) {
                    return;
                }

                if (job.Running) {
                    job.Cancel();
                }

                jobQueue.Remove(job);
            }
        }

        /// <summary>
        /// 重啟主要工作執行緒
        /// </summary>
        public void Reboot() {
            if (IsIdle) {
                loop = Task.Run(() => {
                    while (jobQueue.Count > 0) {
                        var current = jobQueue[0];
                        current.Invoke();

                        try {
                            current.Task.Wait();
                        } catch { }


                        Remove(current);
                    }
                });
            }
        }

        /// <summary>
        /// 等候工作者到執行完成
        /// </summary>
        public void WaitForIdle() {
            if (loop == null || loop.IsCompleted || loop.IsFaulted || loop.IsCanceled) {
                return;
            }
            loop.Wait();
        }
    }
}
