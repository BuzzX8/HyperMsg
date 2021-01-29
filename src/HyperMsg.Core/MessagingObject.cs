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

        protected IMessageHandlersRegistry HandlerRegistry => MessagingContext.HandlersRegistry;

        protected IMessageSender Sender => MessagingContext.Sender;

        protected void RegisterDisposable(IDisposable disposable) => subscriptions.Add(disposable);

        protected void RegisterHandler<TMessage>(Action<TMessage> handler) => RegisterDisposable(HandlerRegistry.RegisterHandler(handler));

        protected void RegisterHandler<TMessage>(AsyncAction<TMessage> handler) => RegisterDisposable(HandlerRegistry.RegisterHandler(handler));

        protected void RegisterReceiveHandler<TMessage>(Action<TMessage> handler) => RegisterDisposable(HandlerRegistry.RegisterReceiveHandler(handler));

        protected void RegisterReceiveHandler<TMessage>(AsyncAction<TMessage> handler) => RegisterDisposable(HandlerRegistry.RegisterReceiveHandler(handler));

        protected void RegisterTransmitHandler<TMessage>(Action<TMessage> handler) => RegisterDisposable(HandlerRegistry.RegisterTransmitHandler(handler));

        protected void RegisterTransmitHandler<TMessage>(AsyncAction<TMessage> handler) => RegisterDisposable(HandlerRegistry.RegisterTransmitHandler(handler));

        protected void Send<TMessage>(TMessage message) => Sender.Send(message);

        protected Task SendAsync<TMessage>(TMessage message, CancellationToken cancellationToken) => Sender.SendAsync(message, cancellationToken);

        protected void Transmit<TMessage>(TMessage message) => Sender.Transmit(message);

        protected Task TransmitAsync<TMessage>(TMessage message, CancellationToken cancellationToken) => Sender.TransmitAsync(message, cancellationToken);

        protected void Received<TMessage>(TMessage message) => Sender.Receive(message);

        protected Task ReceivedAsync<TMessage>(TMessage message, CancellationToken cancellationToken) => Sender.ReceiveAsync(message, cancellationToken);

        public virtual void Dispose() => subscriptions.ForEach(s => s.Dispose());
    }
}
