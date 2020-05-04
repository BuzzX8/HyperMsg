using System;
using System.Collections.Generic;
using System.Threading;

namespace HyperMsg
{
    public abstract class MessagingTaskBase : IDisposable
    {
        private readonly IMessagingContext messagingContext;
        private readonly List<IDisposable> subscriptions;
        private readonly IDisposable cancelSubscription;

        protected MessagingTaskBase(IMessagingContext messagingContext, CancellationToken cancellationToken)
        {
            this.messagingContext = messagingContext ?? throw new ArgumentNullException(nameof(messagingContext));
            CancellationToken = cancellationToken;
            subscriptions = new List<IDisposable>();            
            cancelSubscription = cancellationToken.Register(Dispose);
        }

        protected CancellationToken CancellationToken { get; }

        protected IMessageSender Sender => messagingContext.Sender;

        protected IMessageObservable Observable => messagingContext.Observable;

        protected void RegisterHandler<T>(Action<T> handler) => subscriptions.Add(Observable.Subscribe(handler));

        protected void RegisterHandler<T>(AsyncAction<T> handler) => subscriptions.Add(Observable.Subscribe(handler));

        protected void RegisterReceiveHandler<T>(Action<T> handler) => subscriptions.Add(Observable.OnReceived(handler));

        protected void RegisterReceiveHandler<T>(AsyncAction<T> handler) => subscriptions.Add(Observable.OnReceived(handler));

        public void Dispose()
        {
            cancelSubscription?.Dispose();
            subscriptions.ForEach(d => d.Dispose());
        }
    }
}
