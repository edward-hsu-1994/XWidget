using System;
using System.Threading;
using System.Threading.Tasks;

namespace XWidget.JobQueue {
    /// <summary>
    /// 工作內容
    /// </summary>
    public interface IJob {
        /// <summary>
        /// 工作編號
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// 是否執行中
        /// </summary>
        bool Running { get; }

        /// <summary>
        /// 主程序
        /// </summary>
        Task Task { get; }

        /// <summary>
        /// 終止工作Token
        /// </summary>
        CancellationTokenSource CancellationToken { get; }

        /// <summary>
        /// 引動工作
        /// </summary>
        void Invoke();

        /// <summary>
        /// 取消工作
        /// </summary>
        void Cancel();
    }
}