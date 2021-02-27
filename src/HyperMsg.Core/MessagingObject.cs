using HyperMsg.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public abstract class MessagingObject : IMessageSender, IMessageHandlersRegistry, IDisposable
    {
        private readonly List<IDisposable> subscriptions;

        protected MessagingObject(IMessagingContext messagingContext) => (MessagingContext, subscriptions) = (messagingContext ?? throw new ArgumentNullException(nameof(messagingContext)), new());

        protected IMessagingContext MessagingContext { get; }

        protected IMessageHandlersRegistry HandlersRegistry => MessagingContext.HandlersRegistry;

        protected IMessageSender Sender => MessagingContext.Sender;

        protected void RegisterDisposable(IDisposable disposable) => subscriptions.Add(disposable);

        protected void RegisterDefaultDisposables()
        {
            foreach(var handle in GetDefaultDisposables())
            {
                RegisterDisposable(handle);
            }
        }

        protected virtual IEnumerable<IDisposable> GetDefaultDisposables() => Enumerable.Empty<IDisposable>();

        public IDisposable RegisterHandler<TMessage>(Action<TMessage> handler) => HandlersRegistry.RegisterHandler(handler);

        public IDisposable RegisterHandler<TMessage>(AsyncAction<TMessage> handler) => HandlersRegistry.RegisterHandler(handler);        

        public void Send<T>(T message) => Sender.Send(message);

        public Task SendAsync<T>(T message, CancellationToken cancellationToken) => Sender.SendAsync(message, cancellationToken);        

        public virtual void Dispose() => subscriptions.ForEach(s => s.Dispose());
    }
}
