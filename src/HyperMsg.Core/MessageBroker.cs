using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    /// <summary>
    /// Provides implementation for MessageSender and MessageHandlerRegistry
    /// </summary>
    public class MessageBroker : IMessageSender, IMessageObservable
    {
        private readonly ConcurrentDictionary<Type, object> handlers = new ConcurrentDictionary<Type, object>();
        private readonly ConcurrentDictionary<Type, object> asyncHandlers = new ConcurrentDictionary<Type, object>();

        public void Subscribe<T>(Action<T> handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            if (handlers.ContainsKey(typeof(T)))
            {
                var h = (Action<T>)handlers[typeof(T)];
                h += handler;
                return;
            }

            handlers.TryAdd(typeof(T), handler);
        }

        public void Subscribe<T>(AsyncAction<T> handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            if (asyncHandlers.ContainsKey(typeof(T)))
            {
                var h = (AsyncAction<T>)asyncHandlers[typeof(T)];
                h += handler;
                return;
            }

            asyncHandlers.TryAdd(typeof(T), handler);
        }

        public void Send<T>(T message) => SendAsync(message, CancellationToken.None).GetAwaiter().GetResult();

        public async Task SendAsync<T>(T message, CancellationToken cancellationToken)
        {
            if (handlers.TryGetValue(typeof(T), out var h))
            {
                ((Action<T>)h).Invoke(message);
            }

            if (asyncHandlers.TryGetValue(typeof(T), out var ah))
            {
                await ((AsyncAction<T>)ah).Invoke(message, cancellationToken);
            }
        }
    }
}