using HyperMsg.Messages;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class TopicMessageFilter<T> : IMessageSender
    {
        private readonly IMessageSender parentSender;
        private readonly Func<object, T, bool> filterFunc;

        public TopicMessageFilter(IMessageSender parentSender, Func<object, T, bool> filterFunc = null)
        {
            this.parentSender = parentSender;
            this.filterFunc = filterFunc;
        }

        protected virtual bool ShoudlFilterMessage(object TopicId, T message)
        {
            if (filterFunc == null)
            {
                return false;
            }

            return filterFunc.Invoke(TopicId, message);
        }

        public virtual void Send<TMessage>(TMessage message)
        {
            if (message is TopicMessage<T> TopicMessage && !ShoudlFilterMessage(TopicMessage.TopicId, TopicMessage.Message))
            {
                return;
            }

            parentSender.Send(message);
        }

        public virtual Task SendAsync<TMessage>(TMessage message, CancellationToken cancellationToken)
        {
            if (message is TopicMessage<T> TopicMessage && !ShoudlFilterMessage(TopicMessage.TopicId, TopicMessage.Message))
            {
                return Task.CompletedTask;
            }

            return parentSender.SendAsync(message, cancellationToken);
        }
    }
}
