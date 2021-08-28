using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public abstract class MessageFilter : ISender
    {
        private readonly ISender messageSender;

        protected MessageFilter(ISender messageSender) => 
            this.messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));

        public virtual void Send<T>(T message)
        {
            if (!HandleMessage(ref message))
            {
                return;
            }

            messageSender.Send(message);
        }

        public virtual Task SendAsync<T>(T message, CancellationToken cancellationToken)
        {
            if (!HandleMessage(ref message))
            {
                return Task.CompletedTask;
            }

            return messageSender.SendAsync(message, cancellationToken);
        }

        protected abstract bool HandleMessage<T>(ref T message);
    }
}
