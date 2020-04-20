using System;

namespace HyperMsg
{
    public static class MessageObservableExtensions
    {
        public static IDisposable OnTransmit<T>(this IMessageObservable messageObservable, Action<T> messageObserver)
        {
            return messageObservable.Subscribe<Transmit<T>>(m => messageObserver.Invoke(m));
        }

        public static IDisposable OnTransmit<T>(this IMessageObservable messageObservable, AsyncAction<T> messageObserver)
        {
            return messageObservable.Subscribe<Transmit<T>>((m, t) => messageObserver.Invoke(m, t));
        }

        public static IDisposable OnBufferDataTransmit(this IMessageObservable messageObservable, Action<IBuffer> messageObserver)
        {
            return messageObservable.OnTransmit(messageObserver);
        }

        public static IDisposable OnBufferDataTransmit(this IMessageObservable messageObservable, AsyncAction<IBuffer> messageObserver)
        {
            return messageObservable.OnTransmit(messageObserver);
        }

        public static IDisposable OnReceived<T>(this IMessageObservable messageObservable, Action<T> messageObserver)
        {
            return messageObservable.Subscribe<Received<T>>(m => messageObserver.Invoke(m));
        }

        public static IDisposable OnReceived<T>(this IMessageObservable messageObservable, AsyncAction<T> messageObserver)
        {
            return messageObservable.Subscribe<Received<T>>((m, t) => messageObserver.Invoke(m, t));
        }

        public static IDisposable OnBufferReceivedData(this IMessageObservable messageObservable, Action<IBuffer> messageObserver)
        {
            return messageObservable.OnReceived(messageObserver);
        }

        public static IDisposable OnBufferReceivedData(this IMessageObservable messageObservable, AsyncAction<IBuffer> messageObserver)
        {
            return messageObservable.OnReceived(messageObserver);
        }
    }
}
