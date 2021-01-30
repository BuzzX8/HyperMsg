using System;
using System.Threading.Tasks;

namespace HyperMsg.Extensions
{
    public static class MessageHandlersRegistryExtensions
    {
        public static IDisposable RegisterTransmitHandler<T>(this IMessageHandlersRegistry handlersRegistry, Action<T> messageObserver) => handlersRegistry.RegisterHandler<Transmit>(m =>
        {
            if (typeof(T).IsAssignableFrom(m.Message.GetType()))
            {
                messageObserver.Invoke((T)m.Message);
            }
        });

        public static IDisposable RegisterTransmitHandler<T>(this IMessageHandlersRegistry handlersRegistry, AsyncAction<T> messageObserver) => handlersRegistry.RegisterHandler<Transmit>((m, t) =>
        {
            if (typeof(T).IsAssignableFrom(m.Message.GetType()))
            {
                return messageObserver.Invoke((T)m.Message, t);
            }

            return Task.CompletedTask;
        });        

        public static IDisposable RegisterReceiveHandler<T>(this IMessageHandlersRegistry handlersRegistry, Action<T> messageObserver) => handlersRegistry.RegisterHandler<Received>(m =>
        {
            if (typeof(T).IsAssignableFrom(m.Message.GetType()))
            {
                messageObserver.Invoke((T)m.Message);
            }                
        });

        public static IDisposable RegisterReceiveHandler<T>(this IMessageHandlersRegistry handlersRegistry, AsyncAction<T> messageObserver) => handlersRegistry.RegisterHandler<Received>((m, t) =>
        {                
            if (typeof(T).IsAssignableFrom(m.Message.GetType()))
            {
                return messageObserver.Invoke((T)m.Message, t);
            }

            return Task.CompletedTask;
        });

        public static IDisposable RegisterHandler<T>(this IMessageHandlersRegistry handlersRegistry, Func<T, bool> predicate, Action<T> messageHandler) =>
            handlersRegistry.RegisterHandler<T>(m =>
            {
                if (predicate.Invoke(m))
                {
                    messageHandler.Invoke(m);
                }
            });

        public static IDisposable RegisterHandler<T>(this IMessageHandlersRegistry handlersRegistry, Func<T, bool> predicate, AsyncAction<T> messageHandler) =>
            handlersRegistry.RegisterHandler<T>((m, t) =>
            {
                if (predicate.Invoke(m))
                {
                    return messageHandler.Invoke(m, t);
                }

                return Task.CompletedTask;
            });


        public static IDisposable RegisterHandler<T>(this IMessageHandlersRegistry handlersRegistry, T message, Action<T> messageHandler) where T : IEquatable<T>
        {
            return handlersRegistry.RegisterHandler(m => m.Equals(message), messageHandler);
        }

        public static IDisposable RegisterHandler<T>(this IMessageHandlersRegistry handlersRegistry, T message, AsyncAction<T> messageHandler) where T : IEquatable<T>
        {
            return handlersRegistry.RegisterHandler(m => m.Equals(message), messageHandler);
        }
    }
}
