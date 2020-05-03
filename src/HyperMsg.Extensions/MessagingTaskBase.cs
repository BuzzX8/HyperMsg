using System;
using System.Collections.Generic;
using System.Threading;

namespace HyperMsg
{
    public abstract class MessagingTaskBase : IDisposable
    {
        private readonly IMessageObservable messageObservable;
        private readonly List<IDisposable> subscriptions;
        private IDisposable cancelSubscription;

        protected MessagingTaskBase(IMessageObservable messageObservable)
        {
            this.messageObservable = messageObservable ?? throw new ArgumentNullException(nameof(messageObservable));
            subscriptions = new List<IDisposable>();
        }

        protected void RegisterHandler<T>(Action<T> handler) => subscriptions.Add(messageObservable.Subscribe(handler));

        protected void RegisterHandler<T>(AsyncAction<T> handler) => subscriptions.Add(messageObservable.Subscribe(handler));

        protected void RegisterReceiveHandler<T>(Action<T> handler) => subscriptions.Add(messageObservable.OnReceived(handler));

        protected void RegisterReceiveHandler<T>(AsyncAction<T> handler) => subscriptions.Add(messageObservable.OnReceived(handler));

        protected void SetCancellationToken(CancellationToken cancellationToken) => cancelSubscription = cancellationToken.Register(Dispose);

        public void Dispose()
        {
            cancelSubscription?.Dispose();
            subscriptions.ForEach(d => d.Dispose());
        }
    }
}
