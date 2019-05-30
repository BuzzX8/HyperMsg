using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public abstract class BackgroundWorker : IDisposable
    {
        private CancellationTokenSource tokenSource;
        private Task backgroundTask;

        public bool IsRunning => backgroundTask.Status != TaskStatus.RanToCompletion
            && backgroundTask.Status != TaskStatus.Faulted
            && backgroundTask.Status != TaskStatus.Canceled;

        protected void Run()
        {
            if (backgroundTask != null)
            {
                return;
            }

            tokenSource = new CancellationTokenSource();
            RunBackgroundTask();
        }

        public void Dispose()
        {
            if (tokenSource == null)
            {
                return;
            }

            Stop();
            tokenSource.Dispose();
            tokenSource = null;
        }

        protected void Stop()
        {
            if (tokenSource == null)
            {
                return;
            }

            tokenSource.Cancel();
            backgroundTask = null;
        }

        private void RunBackgroundTask()
        {
            var token = tokenSource.Token;
            backgroundTask = Task.Run(() => DoWorkAsync(token), token);
            backgroundTask.ConfigureAwait(false);
            backgroundTask.ContinueWith(OnBackgroundTaskCompleted);
        }

        private async Task DoWorkAsync(CancellationToken cancellationToken)
        {            
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Run(() => DoWorkIterationAsync(cancellationToken), cancellationToken);
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
