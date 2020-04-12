using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    /// <summary>
    /// Provides implementation for MessageSender and MessageHandlerRegistry
    /// </summary>
    public class MessageBroker : IMessageSender, IMessageObservable
    {
        private class Subscription : IDisposable
        {
            private readonly object observer;
            private readonly List<object> list;

            public Subscription(object observer, List<object> list)
            {
                this.observer = observer;
                this.list = list;
            }

            public void Dispose()
            {
                list.Remove(observer);
            }
        }

        private readonly ConcurrentDictionary<Type, List<object>> observers = new ConcurrentDictionary<Type, List<object>>();
        private readonly ConcurrentDictionary<Type, List<object>> asyncObservers = new ConcurrentDictionary<Type, List<object>>();

        public IDisposable Subscribe<T>(Action<T> messageObserver) => AddObserver<T>(observers, messageObserver);

        public IDisposable Subscribe<T>(AsyncAction<T> messageObserver) => AddObserver<T>(asyncObservers, messageObserver);

        private IDisposable AddObserver<T>(ConcurrentDictionary<Type, List<object>> observers, object messageObserver)
        {
            if (messageObserver == null)
            {
                throw new ArgumentNullException(nameof(messageObserver));
            }

            if (observers.ContainsKey(typeof(T)))
            {
                observers[typeof(T)].Add(messageObserver);
            }
            else
            {
                observers.TryAdd(typeof(T), new List<object> { messageObserver });
            }

            return new Subscription(messageObserver, observers[typeof(T)]);
        }

        public void Send<T>(T message) => SendAsync(message, CancellationToken.None).GetAwaiter().GetResult();

        public async Task SendAsync<T>(T message, CancellationToken cancellationToken)
        {            
            if (this.observers.TryGetValue(typeof(T), out var observers))
            {
                foreach (var observer in observers.Cast<Action<T>>())
                {
                    observer.Invoke(message);
                }
            }

            if (this.asyncObservers.TryGetValue(typeof(T), out var asyncObservers))
            {
                foreach (var asyncObserver in asyncObservers.Cast<AsyncAction<T>>())
                {
                    await asyncObserver.Invoke(message, cancellationToken);
                }
            }
        }
    }
}