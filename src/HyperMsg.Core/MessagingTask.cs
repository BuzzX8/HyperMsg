using HyperMsg.Extensions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public abstract class MessagingTask<TResult> : IDisposable
    {
        private readonly List<IDisposable> subscriptions;
        private readonly TaskCompletionSource<TResult> completionSource;

        protected MessagingTask(IMessagingContext messagingContext, CancellationToken cancellationToken = default)
        {
            CancellationToken = cancellationToken;
            MessagingContext = messagingContext ?? throw new ArgumentNullException(nameof(messagingContext));
            completionSource = new TaskCompletionSource<TResult>();
            subscriptions = new List<IDisposable>();
        }

        public bool IsCompleted => completionSource.Task.IsCompleted;

        public TaskAwaiter<TResult> GetAwaiter() => completionSource.Task.GetAwaiter();

        public Task<TResult> AsTask() => completionSource.Task;

        protected CancellationToken CancellationToken { get; }        

        protected IMessagingContext MessagingContext { get; }

        protected IMessageObservable Observable => MessagingContext.Observable;

        protected IMessageSender Sender => MessagingContext.Sender;

        public TResult Result => completionSource.Task.Result;

        protected void AddHandler<TMessage>(Action<TMessage> handler) => subscriptions.Add(Observable.AddObserver(handler));

        protected void AddHandler<TMessage>(AsyncAction<TMessage> handler) => subscriptions.Add(Observable.AddObserver(handler));

        protected void AddReceiver<TMessage>(Action<TMessage> handler) => subscriptions.Add(Observable.OnReceived(handler));

        protected void AddReceiver<TMessage>(AsyncAction<TMessage> handler) => subscriptions.Add(Observable.OnReceived(handler));

        protected void Send<TMessage>(TMessage message) => Sender.Send(message);

        protected Task SendAsync<TMessage>(TMessage message, CancellationToken cancellationToken) => Sender.SendAsync(message, cancellationToken);

        protected void Transmit<TMessage>(TMessage message) => Sender.Transmit(message);

        protected Task TransmitAsync<TMessage>(TMessage message, CancellationToken cancellationToken) => Sender.TransmitAsync(message, cancellationToken);

        protected void Complete(TResult result)
        {
            completionSource.SetResult(result);
            Dispose();
        }

        public void Dispose() => subscriptions.ForEach(s => s.Dispose());
    }
}
