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

        protected void RegisterHandler<T>(Func<T, bool> predicate, Action messageHandler) => RegisterDisposable(HandlerRegistry.RegisterHandler(predicate, messageHandler));

        protected void RegisterHandler<T>(Func<T, bool> predicate, Action<T> messageHandler) => RegisterDisposable(HandlerRegistry.RegisterHandler(predicate, messageHandler));

        protected void RegisterHandler<T>(Func<T, bool> predicate, AsyncAction messageHandler) => RegisterDisposable(HandlerRegistry.RegisterHandler(predicate, messageHandler));

        protected void RegisterHandler<T>(Func<T, bool> predicate, AsyncAction<T> messageHandler) => RegisterDisposable(HandlerRegistry.RegisterHandler(predicate, messageHandler));

        protected void RegisterHandler<T>(T message, Action messageHandler) => RegisterDisposable(HandlerRegistry.RegisterHandler(message, messageHandler));

        protected void RegisterHandler<T>(T message, Action<T> messageHandler) => RegisterDisposable(HandlerRegistry.RegisterHandler(message, messageHandler));

        protected void RegisterHandler<T>(T message, AsyncAction messageHandler) => RegisterDisposable(HandlerRegistry.RegisterHandler(message, messageHandler));

        protected void RegisterHandler<T>(T message, AsyncAction<T> messageHandler) => RegisterDisposable(HandlerRegistry.RegisterHandler(message, messageHandler));

        protected void RegisterTransmitHandler<T>(Action<T> messageHandler) => RegisterDisposable(HandlerRegistry.RegisterTransmitHandler(messageHandler));

        protected void RegisterTransmitHandler<T>(AsyncAction<T> messageHandler) => RegisterDisposable(HandlerRegistry.RegisterTransmitHandler(messageHandler));

        protected void Send<T>(T message) => Sender.Send(message);

        protected Task SendAsync<T>(T message, CancellationToken cancellationToken) => Sender.SendAsync(message, cancellationToken);

        protected void Transmit<T>(T message) => Sender.Transmit(message);

        protected Task TransmitAsync<T>(T message, CancellationToken cancellationToken) => Sender.TransmitAsync(message, cancellationToken);

        protected void Received<T>(T message) => Sender.Receive(message);

        protected Task ReceivedAsync<T>(T message, CancellationToken cancellationToken) => Sender.ReceiveAsync(message, cancellationToken);

        public virtual void Dispose() => subscriptions.ForEach(s => s.Dispose());
    }
}
