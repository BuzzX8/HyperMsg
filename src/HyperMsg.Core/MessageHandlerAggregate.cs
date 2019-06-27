using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class MessageHandlerAggregate<T> : IMessageHandlerRegistry<T>, IMessageHandler<T>
    {
        private readonly List<IMessageHandler<T>> handlers = new List<IMessageHandler<T>>();

        public void Register(IMessageHandler<T> handler)
        {
            handlers.Add(handler);
        }

        public async Task HandleAsync(T message, CancellationToken cancellationToken)
        {
            foreach (var handler in handlers)
            {
                await handler.HandleAsync(message, cancellationToken);
            }
        }
    }
}