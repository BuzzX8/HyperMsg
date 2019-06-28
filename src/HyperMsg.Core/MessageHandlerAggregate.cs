using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class MessageHandlerAggregate<T> : IMessageHandlerRegistry<T>
    {
        private readonly List<AsyncHandler<T>> handlers = new List<AsyncHandler<T>>();

        public void Register(AsyncHandler<T> handler)
        {
            handlers.Add(handler);
        }

        public async Task HandleAsync(T message, CancellationToken cancellationToken)
        {
            foreach (var handler in handlers)
            {
                await handler.Invoke(message, cancellationToken);
            }
        }
    }
}