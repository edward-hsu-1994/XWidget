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
        private List<Worker> _worker { get; set; } = new List<Worker>();

        /// <summary>
        /// 工作者
        /// </summary>
        public IReadOnlyList<Worker> Workers => _worker.AsReadOnly();

        /// <summary>
        /// 待分配工作列隊
        /// </summary>
        private List<IJob> _jobQueue { get; set; } = new List<IJob>();

        /// <summary>
        /// 工作列隊
        /// </summary>
        public IReadOnlyList<IJob> JobQueue => Workers.SelectMany(x => x.JobQueue).Concat(_jobQueue.AsReadOnly()).ToList().AsReadOnly();

        /// <summary>
        /// 是否閒置中
        /// </summary>
        public bool IsIdle => Workers.All(x => x.IsIdle);

        /// <summary>
        /// 主要工作執行緒中斷且還有剩餘工作
        /// </summary>
        public bool IsDead => Workers.Any(x => x.IsDead);

        /// <summary>
        /// 當完成一件工作時觸發事件
        /// </summary>
        public event WorkCompleteJob OnCompleteJob;

        /// <summary>
        /// 建立工作管理者
        /// </summary>
        /// <param name="workers">工作者數量，至少為1</param>
        public WorkerManager(uint workers = 1) {
            if (workers == 0) throw new ArgumentException(nameof(workers));
            for (int i = 0; i < workers; i++) {
                var work = new Worker();
                work.OnCompleteJob += Work_OnCompleteJob;
                _worker.Add(work);
            }
        }

        private void Work_OnCompleteJob(IWorker worker) {
            AssignJobs(Workers, _jobQueue);
            OnCompleteJob?.Invoke(this);
        }

        /// <summary>
        /// 分配等候中的工作
        /// </summary>
        /// <param name="workers">工作者列表</param>
        /// <param name="jobs">工作列隊</param>
        private protected virtual void AssignJobs(IReadOnlyList<IWorker> workers, List<IJob> jobs) {
            lock (_jobQueue) {
                var targetJob = jobs.FirstOrDefault();
                if (targetJob == null) return;

                var targetWorker = workers.FirstOrDefault(x => x.IsIdle);
                if (targetWorker == null) return;

                jobs.Remove(targetJob);
                targetWorker.Add(targetJob);
            }
        }

        /// <summary>
        /// 新增工作
        /// </summary>
        /// <param name="job">工作</param>
        public void Add(IJob job) {
            _jobQueue.Add(job);
            AssignJobs(Workers, _jobQueue);
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
            lock (_jobQueue) {
                var jobs = _jobQueue.Where(x => x.Id == jobId).ToList();
                foreach (var job in jobs) {
                    _jobQueue.Remove(job);
                }
            }
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
