using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class MessageHandlerAggregate<T> : IMessageHandlerRegistry<T>, IMessageHandler<T>
    {
        private readonly Dictionary<Type, List<object>> handlers = new Dictionary<Type, List<object>>();

        public void Register(IMessageHandler<T> handler)
        {
            if (!handlers.ContainsKey(typeof(T)))
            {
                handlers.Add(typeof(T), new List<object>());
            }

            handlers[typeof(T)].Add(handler);
        }

        public async Task HandleAsync(T message, CancellationToken cancellationToken)
        {
            if (!handlers.ContainsKey(typeof(T)))
            {
                return;
            }

            foreach (var handler in handlers[typeof(T)].Cast<IMessageHandler<T>>())
            {
                await handler.HandleAsync(message, cancellationToken);
            }
        }
    }
}