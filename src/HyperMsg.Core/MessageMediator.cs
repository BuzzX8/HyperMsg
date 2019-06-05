using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class MessageMediator : ISender, IHandlerRegistry
    {
        private readonly Dictionary<Type, List<object>> handlers = new Dictionary<Type, List<object>>();

        public void Register<T>(IHandler<T> handler)
        {
            if (!handlers.ContainsKey(typeof(T)))
            {
                handlers.Add(typeof(T), new List<object>());
            }

            handlers[typeof(T)].Add(handler);
        }

        public void Send<T>(T message)
        {
            if (!handlers.ContainsKey(typeof(T)))
            {
                return;
            }

            foreach (var handler in handlers[typeof(T)].Cast<IHandler<T>>())
            {
                handler.Handle(message);
            }
        }

        public async Task SendAsync<T>(T message, CancellationToken cancellationToken)
        {
            if (!handlers.ContainsKey(typeof(T)))
            {
                return;
            }

            foreach (var handler in handlers[typeof(T)].Cast<IHandler<T>>())
            {
                await handler.HandleAsync(message, cancellationToken);
            }
        }
    }
}
