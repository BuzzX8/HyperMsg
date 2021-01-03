using System;
using System.Threading.Tasks;

namespace HyperMsg.Extensions
{
    public static class MessageObservableExtensions
    {
        public static IDisposable OnTransmit<T>(this IMessageObservable messageObservable, Action<T> messageObserver) => messageObservable.AddObserver<Transmit>(m =>
        {
            if (typeof(T).IsAssignableFrom(m.Message.GetType()))
            {
                messageObserver.Invoke((T)m.Message);
            }
        });

        public static IDisposable OnTransmit<T>(this IMessageObservable messageObservable, AsyncAction<T> messageObserver) => messageObservable.AddObserver<Transmit>((m, t) =>
        {
            if (typeof(T).IsAssignableFrom(m.Message.GetType()))
            {
                return messageObserver.Invoke((T)m.Message, t);
            }

            return Task.CompletedTask;
        });

        public static IDisposable OnBufferDataTransmit(this IMessageObservable messageObservable, Action<IBuffer> messageObserver) => messageObservable.OnTransmit(messageObserver);

        public static IDisposable OnBufferDataTransmit(this IMessageObservable messageObservable, AsyncAction<IBuffer> messageObserver) => messageObservable.OnTransmit(messageObserver);

        public static IDisposable OnReceived<T>(this IMessageObservable messageObservable, Action<T> messageObserver) => messageObservable.AddObserver<Received>(m =>
        {
            if (typeof(T).IsAssignableFrom(m.Message.GetType()))
            {
                messageObserver.Invoke((T)m.Message);
            }                
        });

        public static IDisposable OnReceived<T>(this IMessageObservable messageObservable, AsyncAction<T> messageObserver) => messageObservable.AddObserver<Received>((m, t) =>
        {                
            if (typeof(T).IsAssignableFrom(m.Message.GetType()))
            {
                return messageObserver.Invoke((T)m.Message, t);
            }

            return Task.CompletedTask;
        });


        public static IDisposable OnBufferReceivedData(this IMessageObservable messageObservable, Action<IBuffer> messageObserver) => messageObservable.OnReceived(messageObserver);

        public static IDisposable OnBufferReceivedData(this IMessageObservable messageObservable, AsyncAction<IBuffer> messageObserver) => messageObservable.OnReceived(messageObserver);
    }
}
