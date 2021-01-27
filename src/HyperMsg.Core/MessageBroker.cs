using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    /// <summary>
    /// Provides implementation for MessageSender and MessageHandlerRegistry
    /// </summary>
    public class MessageBroker : IMessageSender, IMessageHandlersRegistry, IMessagingContext
    {
        private class Subscription : IDisposable
        {
            private readonly Delegate observer;
            private readonly Type messageType;
            private readonly Action<Type, Delegate> disposeAction;
                        
            private bool isDisposed;

            public Subscription(Type messageType, Delegate observer, Action<Type, Delegate> disposeAction)
            {
                this.messageType = messageType;
                this.observer = observer;
                this.disposeAction = disposeAction;
                isDisposed = false;
            }

            public void Dispose()
            {
                if (isDisposed)
                {
                    return;
                }

                disposeAction.Invoke(messageType, observer);
                isDisposed = true;
            }
        }

        private readonly Dictionary<Type, Delegate> observers = new();
        private readonly List<Delegate> handlers = new();
        private readonly object sync = new();

        public IMessageSender Sender => this;

        public IMessageHandlersRegistry HandlersRegistry => this;

        public IDisposable RegisterHandler<T>(Action<T> messageObserver) => RegisterHandler<T>((m, t) =>
        {
            messageObserver.Invoke(m);
            return Task.CompletedTask;
        });

        public IDisposable RegisterHandler<T>(AsyncAction<T> messageObserver) => AddObserver<T>((Delegate)messageObserver);

        private IDisposable AddObserver<T>(Delegate messageObserver)
        {
            if (messageObserver == null)
            {
                throw new ArgumentNullException(nameof(messageObserver));
            }

            lock (sync)
            {
                if (observers.ContainsKey(typeof(T)))
                {
                    var observer = observers[typeof(T)];
                    observers[typeof(T)] = Delegate.Combine(messageObserver, observer);
                }
                else
                {
                    observers[typeof(T)] = messageObserver;
                }
            }

            return new Subscription(typeof(T), messageObserver, RemoveObserver);
        }

        public void Send<T>(T message) => SendAsync(message, CancellationToken.None).GetAwaiter().GetResult();

        public async Task SendAsync<T>(T message, CancellationToken cancellationToken)
        {            
            var handlers = GetHandlers(message.GetType());
            
            for(int i = 0; i < handlers.Count; i++)
            {
                try
                {
                    await (Task)handlers[i].DynamicInvoke(message, cancellationToken);
                }
                catch(TargetInvocationException e)
                {
                    throw e.InnerException;
                }
            }
        }        

        private List<Delegate> GetHandlers(Type messageType)
        {
            lock (sync)
            {
                handlers.Clear();

                foreach(var key in observers.Keys)
                {
                    if (key.IsAssignableFrom(messageType))
                    {
                        handlers.Add(observers[key]);
                    }
                }

                return handlers;
            }
        }

        private void RemoveObserver(Type messageType, Delegate observer)
        {
            lock (sync)
            {
                var source = Delegate.Remove(observers[messageType], observer);

                if (source == null)
                {
                    observers.Remove(messageType);
                }
                else
                {
                    observers[messageType] = source;
                }
            }
        }
    }
}