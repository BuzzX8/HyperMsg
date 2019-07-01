using System;
using System.Threading;
using System.Threading.Tasks;
using AsyncAction = System.Func<System.Threading.CancellationToken, System.Threading.Tasks.Task>;

namespace HyperMsg
{
    public class BackgroundWorker : IDisposable
    {
        private readonly AsyncAction asyncAction;

        private CancellationTokenSource tokenSource;
        private Task backgroundTask;

        public BackgroundWorker(AsyncAction asyncAction)
        {
            this.asyncAction = asyncAction ?? throw new ArgumentNullException(nameof(asyncAction));
        }

        public bool IsRunning => backgroundTask.Status != TaskStatus.RanToCompletion
            && backgroundTask.Status != TaskStatus.Faulted
            && backgroundTask.Status != TaskStatus.Canceled;

        public Task HandleTransportEventAsync(TransportEventArgs eventArgs, CancellationToken cancellationToken)
        {
            switch (eventArgs.Event)
            {
                case TransportEvent.Opened:
                    return RunAsync(cancellationToken);

                case TransportEvent.Closed:
                    return StopAsync(cancellationToken);
            }

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            if (tokenSource == null)
            {
                return;
            }

            StopAsync(CancellationToken.None).Wait();
            tokenSource.Dispose();
            tokenSource = null;
        }

        private Task RunAsync(CancellationToken cancellationToken)
        {
            if (backgroundTask != null)
            {
                return Task.CompletedTask;
            }

            tokenSource = new CancellationTokenSource();
            RunBackgroundTask();
            return Task.CompletedTask;
        }

        private Task StopAsync(CancellationToken cancellationToken)
        {
            if (tokenSource == null)
            {
                return Task.CompletedTask;
            }

            tokenSource.Cancel();
            backgroundTask = null;
            return Task.CompletedTask;
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
                await Task.Run(() => asyncAction(cancellationToken), cancellationToken);
            }
        }

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
