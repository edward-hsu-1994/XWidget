using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XWidget.JobQueue {
    /// <summary>
    /// 工作管理者
    /// </summary>
    public class WorkerManager : IWorker {

        /// <summary>
        /// 工作者
        /// </summary>
        private List<Worker> Workers { get; set; } = new List<Worker>();

        /// <summary>
        /// 工作列隊
        /// </summary>
        public IReadOnlyList<IJob> JobQueue => Workers.SelectMany(x => x.JobQueue).ToList().AsReadOnly();

        /// <summary>
        /// 是否閒置中
        /// </summary>
        public bool IsIdle => Workers.All(x => x.IsIdle);

        /// <summary>
        /// 主要工作執行緒中斷且還有剩餘工作
        /// </summary>
        public bool IsDead => Workers.Any(x => x.IsDead);

        /// <summary>
        /// 建立工作管理者
        /// </summary>
        /// <param name="workers">工作者數量，至少為1</param>
        public WorkerManager(uint workers = 1) {
            if (workers == 0) throw new ArgumentException(nameof(workers));
            for (int i = 0; i < workers; i++) {
                Workers.Add(new Worker());
            }
        }

        /// <summary>
        /// 新增工作
        /// </summary>
        /// <param name="job">工作</param>
        public void Add(IJob job) {
            Workers.OrderBy(x => x.JobQueue.Count).First().Add(job);
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
            foreach (var worker in Workers) {
                worker.Remove(jobId);
            }
        }

        /// <summary>
        /// 重啟主要工作執行緒
        /// </summary>
        public void Reboot() {
            foreach (var worker in Workers) {
                worker.Reboot();
            }
        }

        /// <summary>
        /// 等候工作者到執行完成
        /// </summary>
        public void WaitForIdle() {
            List<Task> tasks = new List<Task>();
            foreach (IWorker worker in Workers) {
                tasks.Add(Task.Run(() => {
                    worker.WaitForIdle();
                }));
            }

            Task.WaitAll(tasks.ToArray());

            if (!IsIdle) {
                WaitForIdle();
            }
        }
    }
}
