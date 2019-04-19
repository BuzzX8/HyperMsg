using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class BackgroundReceiver<T> : IDisposable
    {
        private readonly IReceiver<T> messageReceiver;

        private CancellationTokenSource tokenSource;
        private Action<T> messageHandler;        
        private Task backgroundTask;
        private bool tokenSourceDisposed;

        public BackgroundReceiver(IReceiver<T> messageReceiver)
        {
            this.messageReceiver = messageReceiver ?? throw new ArgumentNullException(nameof(messageReceiver));
            backgroundTask = Task.CompletedTask;
        }

        public IDisposable Run(Action<T> messageHandler)
        {
            this.messageHandler = messageHandler ?? throw new ArgumentNullException(nameof(messageHandler));
            tokenSource = new CancellationTokenSource();
            backgroundTask = RunBackgroundTask(tokenSource.Token);

            return this;
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

        private Task RunBackgroundTask(CancellationToken token)
        {
            return Task.Run(() => DoWorkAsync(token), tokenSource.Token).ContinueWith(OnBackgroundTaskFault, TaskContinuationOptions.OnlyOnFaulted);            
        }

        private async Task DoWorkAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var message = await messageReceiver.ReceiveAsync(token);
                messageHandler.Invoke(message);
            }
        }

        private void OnBackgroundTaskFault(Task task)
        {
            var exception = task.Exception.Flatten()?.InnerException;
            OnUnhandlerException?.Invoke(exception);
        }

        public Action<Exception> OnUnhandlerException;
    }
}
