using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public abstract class MessagingContextProxy : IMessageSender, IMessageHandlersRegistry
    {
        protected MessagingContextProxy(IMessagingContext messagingContext) => (MessagingContext) = (messagingContext ?? throw new ArgumentNullException(nameof(messagingContext)));

        private IMessagingContext MessagingContext { get; }

        private IMessageHandlersRegistry HandlersRegistry => MessagingContext.HandlersRegistry;

        private IMessageSender Sender => MessagingContext.Sender;

        protected virtual IEnumerable<IDisposable> GetAutoDisposables() => Enumerable.Empty<IDisposable>();

        public virtual IDisposable RegisterHandler<TMessage>(Action<TMessage> handler) => HandlersRegistry.RegisterHandler(handler);

        public virtual IDisposable RegisterHandler<TMessage>(AsyncAction<TMessage> handler) => HandlersRegistry.RegisterHandler(handler);        

        public virtual void Send<T>(T message) => Sender.Send(message);

        public virtual Task SendAsync<T>(T message, CancellationToken cancellationToken) => Sender.SendAsync(message, cancellationToken);
    }
}
