using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class BackgroundReceiver<T> : IDisposable
    {
        private readonly IReceiver<T> messageReceiver;

        private CancellationTokenSource tokenSource;        
        private Task backgroundTask;
        private bool tokenSourceDisposed;

        public BackgroundReceiver(IReceiver<T> messageReceiver)
        {
            this.messageReceiver = messageReceiver ?? throw new ArgumentNullException(nameof(messageReceiver));
            backgroundTask = Task.CompletedTask;
        }

        public bool IsRunning => backgroundTask.Status != TaskStatus.RanToCompletion
            && backgroundTask.Status != TaskStatus.Faulted
            && backgroundTask.Status != TaskStatus.Canceled;

        public void Run()
        {            
            tokenSource = new CancellationTokenSource();
            tokenSourceDisposed = false;
            backgroundTask = RunBackgroundTask(tokenSource.Token);
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

        public void Stop()
        {
            Dispose();
            backgroundTask.Wait();            
        }

        public bool Stop(TimeSpan waitTimeout)
        {
            Dispose();
            return backgroundTask.Wait(waitTimeout);
        }

        private Task RunBackgroundTask(CancellationToken token)
        {
            return Task.Run(DoWorkAsync).ContinueWith(OnBackgroundTaskFault, TaskContinuationOptions.OnlyOnFaulted);            
        }

        private async Task DoWorkAsync()
        {
            var token = tokenSource.Token;

            while (!token.IsCancellationRequested)
            {
                var message = await messageReceiver.ReceiveAsync(token);
                OnMessageReceived(message);
            }
        }

        private void OnMessageReceived(T message)
        {
            MessageReceived?.Invoke(message);
        }

        private void OnBackgroundTaskFault(Task task)
        {
            var exception = task.Exception.Flatten()?.InnerException;
            OnUnhandlerException?.Invoke(exception);
        }

        public Action<T> MessageReceived;

        public Action<Exception> OnUnhandlerException;
    }
}
