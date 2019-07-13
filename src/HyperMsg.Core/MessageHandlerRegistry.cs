using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class MessageHandlerRegistry<T> : IMessageHandlerRegistry<T>
    {
        private Action<T> handlers;
        private AsyncAction<T> asyncHandlers;

        public void Register(Action<T> handler) => handlers += handler;

        public void Register(AsyncAction<T> handler) => asyncHandlers += handler;

        public async Task HandleAsync(T message, CancellationToken cancellationToken)
        {
            handlers?.Invoke(message);

            if (asyncHandlers != null)
            {
                await asyncHandlers.Invoke(message, cancellationToken);
            }
        }
    }
}