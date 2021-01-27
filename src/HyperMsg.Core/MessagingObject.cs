using HyperMsg.Extensions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public abstract class MessagingObject : IDisposable
    {
        private readonly List<IDisposable> subscriptions;

        protected MessagingObject(IMessagingContext messagingContext) => (MessagingContext, subscriptions) = (messagingContext ?? throw new ArgumentNullException(nameof(messagingContext)), new());

        protected IMessagingContext MessagingContext { get; }

        protected IMessageHandlersRegistry Observable => MessagingContext.HandlersRegistry;

        protected IMessageSender Sender => MessagingContext.Sender;

        protected void AddHandler<TMessage>(Action<TMessage> handler) => subscriptions.Add(Observable.RegisterHandler(handler));

        protected void AddHandler<TMessage>(AsyncAction<TMessage> handler) => subscriptions.Add(Observable.RegisterHandler(handler));

        protected void AddReceiver<TMessage>(Action<TMessage> handler) => subscriptions.Add(Observable.RegisterReceiveHandler(handler));

        protected void AddReceiver<TMessage>(AsyncAction<TMessage> handler) => subscriptions.Add(Observable.RegisterReceiveHandler(handler));

        protected void AddTransmitter<TMessage>(Action<TMessage> handler) => subscriptions.Add(Observable.RegisterTransmitHandler(handler));

        protected void AddTransmitter<TMessage>(AsyncAction<TMessage> handler) => subscriptions.Add(Observable.RegisterTransmitHandler(handler));

        protected void Send<TMessage>(TMessage message) => Sender.Send(message);

        protected Task SendAsync<TMessage>(TMessage message, CancellationToken cancellationToken) => Sender.SendAsync(message, cancellationToken);

        protected void Transmit<TMessage>(TMessage message) => Sender.Transmit(message);

        protected Task TransmitAsync<TMessage>(TMessage message, CancellationToken cancellationToken) => Sender.TransmitAsync(message, cancellationToken);

        protected void Received<TMessage>(TMessage message) => Sender.Receive(message);

        protected Task ReceivedAsync<TMessage>(TMessage message, CancellationToken cancellationToken) => Sender.ReceiveAsync(message, cancellationToken);

        public virtual void Dispose() => subscriptions.ForEach(s => s.Dispose());
    }
}
