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
        private readonly ConcurrentDictionary<Type, object> observers = new ConcurrentDictionary<Type, object>();
        private readonly ConcurrentDictionary<Type, object> asyncObservers = new ConcurrentDictionary<Type, object>();

        public void Subscribe<T>(Action<T> messageObserver)
        {
            if (messageObserver == null)
            {
                throw new ArgumentNullException(nameof(messageObserver));
            }

            if (observers.ContainsKey(typeof(T)))
            {
                var observers = (Action<T>)this.observers[typeof(T)];
                observers += messageObserver;
                return;
            }

            observers.TryAdd(typeof(T), messageObserver);
        }

        public void Subscribe<T>(AsyncAction<T> messageObserver)
        {
            if (messageObserver == null)
            {
                throw new ArgumentNullException(nameof(messageObserver));
            }

            if (asyncObservers.ContainsKey(typeof(T)))
            {
                var asyncObservers = (AsyncAction<T>)this.asyncObservers[typeof(T)];
                asyncObservers += messageObserver;
                return;
            }

            asyncObservers.TryAdd(typeof(T), messageObserver);
        }

        public void Send<T>(T message) => SendAsync(message, CancellationToken.None).GetAwaiter().GetResult();

        public async Task SendAsync<T>(T message, CancellationToken cancellationToken)
        {
            if (this.observers.TryGetValue(typeof(T), out var observers))
            {
                ((Action<T>)observers).Invoke(message);
            }

            if (this.asyncObservers.TryGetValue(typeof(T), out var asyncObservers))
            {
                await ((AsyncAction<T>)asyncObservers).Invoke(message, cancellationToken);
            }
        }
    }
}