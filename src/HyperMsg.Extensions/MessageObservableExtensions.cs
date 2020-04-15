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

        public static IDisposable OnReceive<T>(this IMessageObservable messageObservable, Action<T> messageObserver)
        {
            return messageObservable.Subscribe<Received<T>>(m => messageObserver.Invoke(m));
        }

        public static IDisposable OnReceive<T>(this IMessageObservable messageObservable, AsyncAction<T> messageObserver)
        {
            return messageObservable.Subscribe<Received<T>>((m, t) => messageObserver.Invoke(m, t));
        }
    }
}
