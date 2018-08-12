using System;
using System.Collections.Generic;
using System.Text;

namespace XWidget.JobQueue {
    /// <summary>
    /// 工作者介面
    /// </summary>
    public interface IWorker {
        /// <summary>
        /// 工作列隊
        /// </summary>
        IReadOnlyList<IJob> JobQueue { get; }

        /// <summary>
        /// 是否閒置中
        /// </summary>
        bool IsIdle { get; }

        /// <summary>
        /// 主要工作執行緒中斷且還有剩餘工作
        /// </summary>
        bool IsDead { get; }

        /// <summary>
        /// 新增工作
        /// </summary>
        /// <param name="job">工作</param>
        void Add(IJob job);

        /// <summary>
        /// 取消工作
        /// </summary>
        /// <param name="job">工作</param>
        void Remove(IJob job);

        /// <summary>
        /// 取消工作
        /// </summary>
        /// <param name="jobId">工作唯一識別號</param>
        void Remove(Guid jobId);

        /// <summary>
        /// 重啟主要工作執行緒
        /// </summary>
        void Reboot();

        /// <summary>
        /// 等候工作者到執行完成
        /// </summary>
        void WaitForIdle();
    }
}
