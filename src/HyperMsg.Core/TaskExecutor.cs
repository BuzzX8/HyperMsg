using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class TaskExecutor : ITaskExecutor
    {
        public Task ExecuteAsync(ITask task, CancellationToken cancellationToken)
        {
            var tsc = new CompletionSource(task, cancellationToken);
            return task.InitializeAsync(tsc, cancellationToken).ContinueWith(t => tsc.Task);
        }

        public async Task<T> ExecuteAsync<T>(ITask<T> task, CancellationToken cancellationToken)
        {
            var tsc = new CompletionSource<T>(task);
            await task.InitializeAsync(tsc, cancellationToken);
            return await tsc.Task;
        }
    }

    internal class CompletionSource : ITaskCompletionSource
    {
        private readonly ITask task;
        private readonly IDisposable cancelSubscription;
        private readonly TaskCompletionSource<bool> tsc;

        internal CompletionSource(ITask task, CancellationToken cancellationToken)
        {
            this.task = task;
            tsc = new TaskCompletionSource<bool>();
            cancelSubscription = cancellationToken.Register(() =>
            {
                tsc.SetCanceled();
                this.task.Dispose();
            });
        }

        internal Task Task => tsc.Task;

        public void SetCanceled()
        {
            tsc.SetCanceled();
            cancelSubscription.Dispose();
            task.Dispose();
        }

        public void SetCompleted()
        {
            tsc.SetResult(true);
            cancelSubscription.Dispose();
            task.Dispose();
        }

        public void SetException(Exception exception)
        {
            tsc.SetException(exception);
            cancelSubscription.Dispose();
            task.Dispose();
        }
    }

    internal class CompletionSource<T> : ITaskCompletionSource<T>
    {
        private readonly ITask<T> task;
        private readonly TaskCompletionSource<T> tsc;

        internal CompletionSource(ITask<T> task)
        {
            this.task = task;
            tsc = new TaskCompletionSource<T>();
        }

        internal Task<T> Task => tsc.Task;

        public void SetCanceled()
        {
            throw new NotImplementedException();
        }

        public void SetException(Exception exception)
        {
            throw new NotImplementedException();
        }

        public void SetResult(T result)
        {
            tsc.SetResult(result);
            task.Dispose();
        }
    }
}
