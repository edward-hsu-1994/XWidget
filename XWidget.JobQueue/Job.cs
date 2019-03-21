using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace XWidget.JobQueue {
    /// <summary>
    /// 基本工作
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Job<T> : IJob, IObservable<T>, IDisposable {
        private Func<Job<T>, T> Content { get; set; }

        private List<IObserver<T>> Observers { get; set; } = new List<IObserver<T>>();

        private IDisposable Subscriber { get; set; }

        /// <summary>
        /// 工作編號
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// 是否執行中
        /// </summary>
        public bool Running => Task?.Status == TaskStatus.Running;

        /// <summary>
        /// 主程序
        /// </summary>
        public Task Task { get; private set; }

        /// <summary>
        /// 終止工作Token
        /// </summary>
        public CancellationTokenSource CancellationToken { get; private set; }

        /// <summary>
        /// 結果
        /// </summary>
        public T Result { get; set; }

        CancellationTokenSource IJob.CancellationToken => this.CancellationToken;

        /// <summary>
        /// 建立工作
        /// </summary>
        /// <param name="content">工作內容</param>
        public Job(Func<Job<T>, T> content) {
            Id = Guid.NewGuid();
            Content = content;
        }

        /// <summary>
        /// 引動工作
        /// </summary>
        public void Invoke() {
            CancellationToken = new CancellationTokenSource();

            var taskCompletionSource = new TaskCompletionSource<int>();

            Task = taskCompletionSource.Task;

            Subscriber = Observable.Start<T>(() => {
                return Content(this);
            }).Subscribe((T result) => {
                Parallel.ForEach(Observers, observer => {
                    observer.OnNext(result);
                });
            }, (Exception e) => {
                Parallel.ForEach(Observers, observer => {
                    observer.OnError(e);
                });
                taskCompletionSource.TrySetException(e);
            },
            () => {
                Parallel.ForEach(Observers, observer => {
                    observer.OnCompleted();
                });
                taskCompletionSource.TrySetResult(0);
            });
        }

        /// <summary>
        /// 取消工作
        /// </summary>
        public void Cancel() {
            Subscriber.Dispose();

            Parallel.ForEach(Observers, observer => {
                observer.OnError(new OperationCanceledException());
            });
        }

        /// <summary>
        /// 訂閱
        /// </summary>
        /// <param name="observer"></param>
        /// <returns></returns>
        public IDisposable Subscribe(IObserver<T> observer) {
            Observers.Add(observer);

            return this;
        }

        public void Dispose() {
            Subscriber?.Dispose();
        }
    }
}
