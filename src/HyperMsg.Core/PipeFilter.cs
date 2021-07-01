using HyperMsg.Messages;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class PipeFilter<T> : IMessageSender
    {
        private readonly IMessageSender parentSender;
        private readonly Func<object, object, T, bool> filterFunc;

        public PipeFilter(IMessageSender parentSender, Func<object, object, T, bool> filterFunc = null)
        {
            this.parentSender = parentSender;
            this.filterFunc = filterFunc;
        }

        protected virtual bool ShoudlFilterMessage(object pipeId, object portId, T message)
        {
            if (filterFunc == null)
            {
                return false;
            }

            return filterFunc.Invoke(pipeId, portId, message);
        }

        public virtual void Send<TMessage>(TMessage message)
        {
            if (message is PipeMessage<T> pipeMessage && !ShoudlFilterMessage(pipeMessage.PipeId, pipeMessage.PortId, pipeMessage.Message))
            {
                return;
            }

            parentSender.Send(message);
        }

        public virtual Task SendAsync<TMessage>(TMessage message, CancellationToken cancellationToken)
        {
            if (message is PipeMessage<T> pipeMessage && !ShoudlFilterMessage(pipeMessage.PipeId, pipeMessage.PortId, pipeMessage.Message))
            {
                return Task.CompletedTask;
            }

            return parentSender.SendAsync(message, cancellationToken);
        }
    }
}
