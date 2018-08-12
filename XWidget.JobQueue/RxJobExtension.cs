using System;
using System.Collections.Generic;
using System.Text;

namespace XWidget.JobQueue {
    public static class RxJobExtension {
        /// <summary>
        /// 嘗試轉換為<see cref="IJob"/>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static IJob AsJob(this IDisposable job) {
            return job as IJob;
        }

        /// <summary>
        /// 新增工作
        /// </summary>
        /// <param name="worker">工作者</param>
        /// <param name="job">工作</param>
        public static void Add(this IWorker worker, IDisposable job) {
            if (!(job is IJob)) throw new ArgumentException(nameof(job));
            worker.Add(job.AsJob());
        }

        /// <summary>
        /// 取消工作
        /// </summary>
        /// <param name="worker">工作者</param>
        /// <param name="job">工作</param>
        public static void Remove(this IWorker worker, IDisposable job) {
            if (!(job is IJob)) throw new ArgumentException(nameof(job));
            worker.Remove(job.AsJob().Id);
        }
    }
}
