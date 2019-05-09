using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class BackgroundReceiver<T> : IDisposable, IHandler<ReceiveMode>
    {
        private readonly IReceiver<T> messageReceiver;
        private readonly IEnumerable<IHandler<T>> messageHandlers;

        private CancellationTokenSource tokenSource;        
        private Task backgroundTask;
        private bool tokenSourceDisposed;

        public BackgroundReceiver(IReceiver<T> messageReceiver, IEnumerable<IHandler<T>> messageHandlers)
        {
            this.messageReceiver = messageReceiver ?? throw new ArgumentNullException(nameof(messageReceiver));
            this.messageHandlers = messageHandlers ?? throw new ArgumentNullException(nameof(messageHandlers));
            backgroundTask = Task.CompletedTask;
        }

        public bool IsRunning => backgroundTask.Status != TaskStatus.RanToCompletion
            && backgroundTask.Status != TaskStatus.Faulted
            && backgroundTask.Status != TaskStatus.Canceled;

        public void Run()
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

        private Task RunBackgroundTask()
        {
            return Task.Run(DoWorkAsync).ContinueWith(OnBackgroundTaskCompleted);
        }

        private async Task DoWorkAsync()
        {
            var token = tokenSource.Token;

            while (!token.IsCancellationRequested)
            {
                var message = await messageReceiver.ReceiveAsync(token);
                
                foreach (var handler in messageHandlers)
                {
                    await handler.HandleAsync(message, token);
                }
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

        public void Handle(ReceiveMode message)
        {
            switch (message)
            {
                case ReceiveMode.Proactive:
                    Stop();
                    break;

                case ReceiveMode.Reactive:
                    Run();
                    break;
            }
        }

        public Task HandleAsync(ReceiveMode message, CancellationToken token = default)
        {
            Handle(message);
            return Task.CompletedTask;
        }

        public Action<T> MessageReceived;

        public Action<Task> BackgroundTaskCompleted;

        public Action<Exception> UnhandledException;
    }
}