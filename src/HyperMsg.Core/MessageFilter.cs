using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public abstract class MessageFilter : ISender
    {
        private readonly ISender sender;

        protected MessageFilter(ISender messageSender) => 
            this.sender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));

        public virtual void Send<T>(T message)
        {
            if (!HandleMessage(message))
            {
                return;
            }

            sender.Send(message);
        }

        public virtual Task SendAsync<T>(T message, CancellationToken cancellationToken)
        {
            if (!HandleMessage(message))
            {
                return Task.CompletedTask;
            }

            return sender.SendAsync(message, cancellationToken);
        }

        protected abstract bool HandleMessage<T>(T message);

        protected virtual Task<bool> HandleMessageAsync<T>(T message, CancellationToken _) => Task.FromResult(HandleMessage(message));
    }
}
