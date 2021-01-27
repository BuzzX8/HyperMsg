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
    }
}
