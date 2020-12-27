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
    public class MessageBroker : IMessageSender, IMessageObservable, IMessagingContext
    {
        private class Subscription : IDisposable
        {
            private readonly Delegate observer;
            private readonly List<Delegate> list;

            public Subscription(Delegate observer, List<Delegate> list)
            {
                this.observer = observer;
                this.list = list;
            }

            public void Dispose()
            {
                list.Remove(observer);
            }
        }

        private readonly ConcurrentDictionary<Type, List<Delegate>> observers = new();
        private readonly ConcurrentDictionary<Type, List<Delegate>> asyncObservers = new();

        public IMessageSender Sender => this;

        public IMessageObservable Observable => this;

        public IDisposable Subscribe<T>(Action<T> messageObserver) => AddObserver<T>(observers, messageObserver);

        public IDisposable Subscribe<T>(AsyncAction<T> messageObserver) => AddObserver<T>(asyncObservers, messageObserver);

        private IDisposable AddObserver<T>(ConcurrentDictionary<Type, List<Delegate>> observers, Delegate messageObserver)
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
                observers.TryAdd(typeof(T), new List<Delegate> { messageObserver });
            }

            return new Subscription(messageObserver, observers[typeof(T)]);
        }

        public void Send<T>(T message) => SendAsync(message, CancellationToken.None).GetAwaiter().GetResult();

        public async Task SendAsync<T>(T message, CancellationToken cancellationToken)
        {            
            var handlers = GetHandlers(observers, message.GetType());
            
            foreach(var handler in handlers)
            {
                handler.DynamicInvoke(message);
            }

            handlers = GetHandlers(asyncObservers, message.GetType());

            var tasks = handlers.Select(h => h.DynamicInvoke(message, cancellationToken)).Cast<Task>();
            await Task.WhenAll(tasks);
        }

        private IEnumerable<Delegate> GetHandlers(ConcurrentDictionary<Type, List<Delegate>> observers, Type messageType)
        {
            var keys = observers.Keys.Where(k => k.IsAssignableFrom(messageType));
            return observers.Where(kvp => keys.Contains(kvp.Key))
                .SelectMany(kvp => kvp.Value)
                .ToArray();
        }
    }
}