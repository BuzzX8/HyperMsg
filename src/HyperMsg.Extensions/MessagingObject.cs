using System;
using System.Collections.Generic;

namespace HyperMsg
{
    public abstract class MessagingObject : IDisposable
    {
        private readonly IMessagingContext messagingContext;
        private readonly List<IDisposable> subscriptions;

        protected MessagingObject(IMessagingContext messagingContext)
        {
            this.messagingContext = messagingContext ?? throw new ArgumentNullException(nameof(messagingContext));
            subscriptions = new List<IDisposable>();
        }

        protected IMessageSender Sender => messagingContext.Sender;

        protected IMessageObservable Observable => messagingContext.Observable;

        protected void RegisterHandler<T>(Action<T> handler) => subscriptions.Add(Observable.Subscribe(handler));

        protected void RegisterHandler<T>(AsyncAction<T> handler) => subscriptions.Add(Observable.Subscribe(handler));

        protected void RegisterReceiveHandler<T>(Action<T> handler) => subscriptions.Add(Observable.OnReceived(handler));

        protected void RegisterReceiveHandler<T>(AsyncAction<T> handler) => subscriptions.Add(Observable.OnReceived(handler));

        protected void RegisterTransmitHandler<T>(Action<T> handler) => subscriptions.Add(Observable.OnTransmit(handler));

        protected void RegisterTransmitHandler<T>(AsyncAction<T> handler) => subscriptions.Add(Observable.OnTransmit(handler));

        public virtual void Dispose() => subscriptions.ForEach(d => d.Dispose());
    }
}
