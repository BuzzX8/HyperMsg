using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    /// <summary>
    /// Provides implementation for MessageSender and MessageHandlerRegistry
    /// </summary>
    public class MessageBroker : ISender, IHandlersRegistry
    {
        private class Subscription : IDisposable
        {
            private readonly Delegate messageHandler;
            private readonly Type messageType;
            private readonly Action<Type, Delegate> disposeAction;
                        
            private bool isDisposed;

            public Subscription(Type messageType, Delegate messageHandler, Action<Type, Delegate> disposeAction)
            {
                this.messageType = messageType;
                this.messageHandler = messageHandler;
                this.disposeAction = disposeAction;
                isDisposed = false;
            }

            public void Dispose()
            {
                if (isDisposed)
                {
                    return;
                }

                disposeAction.Invoke(messageType, messageHandler);
                isDisposed = true;
            }
        }

        private readonly ConcurrentDictionary<Type, Delegate> messageHandlers = new();        
        private readonly object sync = new();

        public ISender Sender => this;

        public IHandlersRegistry HandlersRegistry => this;

        public IDisposable RegisterHandler<T>(Action<T> messageHandler) => RegisterHandler<T>((m, t) =>
        {
            messageHandler.Invoke(m);
            return Task.CompletedTask;
        });

        public IDisposable RegisterHandler<T>(AsyncAction<T> messageHandler) => AddHandler<T>(messageHandler);

        private IDisposable AddHandler<T>(Delegate messageHandler)
        {
            if (messageHandler == null)
            {
                throw new ArgumentNullException(nameof(messageHandler));
            }

            lock (sync)
            {
                if (messageHandlers.TryGetValue(typeof(T), out var handler))
                {                    
                    messageHandlers[typeof(T)] = Delegate.Combine(messageHandler, handler);
                }
                else
                {
                    messageHandlers[typeof(T)] = messageHandler;
                }
            }

            return new Subscription(typeof(T), messageHandler, RemoveHandler);
        }

        public void Send<T>(T message) => SendAsync(message, CancellationToken.None).GetAwaiter().GetResult();

        public async Task SendAsync<T>(T message, CancellationToken cancellationToken)
        {   
            if (!messageHandlers.ContainsKey(typeof(T)))
            {
                return;
            }

            if (!messageHandlers.TryGetValue(typeof(T), out var handler))
            {
                return;
            }

            try
            {
                await ((AsyncAction<T>)handler).Invoke(message, cancellationToken);
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException;
            }            
        }

        private void RemoveHandler(Type messageType, Delegate handler)
        {
            lock (sync)
            {
                var source = Delegate.Remove(messageHandlers[messageType], handler);

                if (source == null)
                {
                    messageHandlers.TryRemove(messageType, out var value);
                    return;
                }
                else
                {
                    messageHandlers[messageType] = source;
                }
            }
        }
    }
}