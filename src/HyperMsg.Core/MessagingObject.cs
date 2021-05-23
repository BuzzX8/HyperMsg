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

        private IMessagingContext MessagingContext { get; }

        private IMessageHandlersRegistry HandlersRegistry => MessagingContext.HandlersRegistry;

        private IMessageSender Sender => MessagingContext.Sender;

        private void RegisterDisposable(IDisposable disposable) => subscriptions.Add(disposable);

        protected void RegisterAutoDisposables()
        {
            foreach(var handle in GetAutoDisposables())
            {
                RegisterDisposable(handle);
            }
        }

        protected virtual IEnumerable<IDisposable> GetAutoDisposables() => Enumerable.Empty<IDisposable>();

        public IDisposable RegisterHandler<TMessage>(Action<TMessage> handler) => HandlersRegistry.RegisterHandler(handler);

        public IDisposable RegisterHandler<TMessage>(AsyncAction<TMessage> handler) => HandlersRegistry.RegisterHandler(handler);        

        public void Send<T>(T message) => Sender.Send(message);

        public Task SendAsync<T>(T message, CancellationToken cancellationToken) => Sender.SendAsync(message, cancellationToken);        

        public virtual void Dispose() => subscriptions.ForEach(s => s.Dispose());
    }
}
