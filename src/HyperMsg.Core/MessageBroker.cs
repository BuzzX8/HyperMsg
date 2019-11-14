using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class MessageBroker : IMessageSender, IMessageHandlerRegistry
    {
        private readonly List<object> handlers = new List<object>();
        private readonly List<object> asyncHandlers = new List<object>();

        public void Register<T>(Action<T> handler) => handlers.Add(handler);

        public void Register<T>(AsyncAction<T> handler) => asyncHandlers.Add(handler);

        public void Send<T>(T message) => SendAsync(message, CancellationToken.None).GetAwaiter().GetResult();

        public async Task SendAsync<T>(T message, CancellationToken cancellationToken)
        {
            foreach (var handler in handlers.OfType<Action<T>>())
            {
                handler.Invoke(message);
            }

            foreach(var handler in asyncHandlers.OfType<AsyncAction<T>>())
            {
                await handler.Invoke(message, cancellationToken);
            }
        }
    }
}