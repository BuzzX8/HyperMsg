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
        private readonly IMessagingContext context;
        private readonly List<IDisposable> subscriptions;
        private readonly TaskCompletionSource<TResult> completionSource;

        protected MessagingTask(IMessagingContext context, CancellationToken cancellationToken = default)
        {
            CancellationToken = cancellationToken;
            this.context = context;
            completionSource = new TaskCompletionSource<TResult>();
            subscriptions = new List<IDisposable>();
        }

        public bool IsCompleted => completionSource.Task.IsCompleted;

        public TaskAwaiter<TResult> GetAwaiter() => completionSource.Task.GetAwaiter();

        public Task<TResult> AsTask() => completionSource.Task;

        protected CancellationToken CancellationToken { get; }

        public TResult Result => completionSource.Task.Result;

        protected void AddHandler<TMessage>(Action<TMessage> handler) => subscriptions.Add(context.Observable.Subscribe(handler));

        protected void AddHandler<TMessage>(AsyncAction<TMessage> handler) => subscriptions.Add(context.Observable.Subscribe(handler));

        protected void AddReceiver<TMessage>(Action<TMessage> handler) => subscriptions.Add(context.Observable.OnReceived(handler));

        protected void AddReceiver<TMessage>(AsyncAction<TMessage> handler) => subscriptions.Add(context.Observable.OnReceived(handler));

        protected void Send<TMessage>(TMessage message) => context.Sender.Send(message);

        protected Task SendAsync<TMessage>(TMessage message, CancellationToken cancellationToken) => context.Sender.SendAsync(message, cancellationToken);

        protected void Transmit<TMessage>(TMessage message) => context.Sender.Transmit(message);

        protected Task TransmitAsync<TMessage>(TMessage message, CancellationToken cancellationToken) => context.Sender.TransmitAsync(message, cancellationToken);

        protected void Complete(TResult result)
        {
            completionSource.SetResult(result);
            Dispose();
        }

        public void Dispose() => subscriptions.ForEach(s => s.Dispose());
    }
}
