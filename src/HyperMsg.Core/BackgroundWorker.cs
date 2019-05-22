using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public abstract class BackgroundWorker : IDisposable
    {
        private CancellationTokenSource tokenSource;
        private Task backgroundTask;
        private bool tokenSourceDisposed;

        protected BackgroundWorker()
        {
            backgroundTask = Task.CompletedTask;
        }

        public bool IsRunning => backgroundTask.Status != TaskStatus.RanToCompletion
            && backgroundTask.Status != TaskStatus.Faulted
            && backgroundTask.Status != TaskStatus.Canceled;

        protected void Run()
        {
            if (IsRunning)
            {
                return;
            }

            tokenSource = new CancellationTokenSource();
            tokenSourceDisposed = false;
            backgroundTask = RunBackgroundTask();
        }

        public void Dispose()
        {
            if (tokenSourceDisposed)
            {
                return;
            }

            tokenSource?.Cancel();
            tokenSource?.Dispose();
            tokenSourceDisposed = true;
        }

        protected void Stop()
        {
            Dispose();
            backgroundTask.Wait();
        }

        protected bool Stop(TimeSpan waitTimeout)
        {
            Dispose();
            return backgroundTask.Wait(waitTimeout);
        }

        private Task RunBackgroundTask()
        {
            return Task.Run(DoWorkAsync).ContinueWith(OnBackgroundTaskCompleted);
        }

        private async Task DoWorkAsync()
        {
            var token = tokenSource.Token;

            while (!token.IsCancellationRequested)
            {
                await DoWorkIterationAsync(token);
            }
        }

        protected abstract Task DoWorkIterationAsync(CancellationToken cancellationToken);

        private void OnBackgroundTaskCompleted(Task task)
        {
            BackgroundTaskCompleted?.Invoke(task);

            if (task.Status == TaskStatus.Faulted)
            {
                OnBackgroundTaskFault(task);
            }
        }

        private void OnBackgroundTaskFault(Task task)
        {
            var exception = task.Exception.Flatten()?.InnerException;
            UnhandledException?.Invoke(exception);
        }

        public Action<Task> BackgroundTaskCompleted;

        public Action<Exception> UnhandledException;
    }
}
