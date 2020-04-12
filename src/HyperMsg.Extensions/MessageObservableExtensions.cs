using System;

namespace HyperMsg
{
    public static class MessageObservableExtensions
    {
        public static void SubscribeTransmitter<T>(this IMessageObservable messageObservable, Action<T> messageObserver)
        {
            messageObservable.Subscribe<Transmit<T>>(m => messageObserver.Invoke(m));
        }

        public static void SubscribeTransmitter<T>(this IMessageObservable messageObservable, AsyncAction<T> messageObserver)
        {
            messageObservable.Subscribe<Transmit<T>>((m, t) => messageObserver.Invoke(m, t));
        }

        public static void SubscribeReceiver<T>(this IMessageObservable messageObservable, Action<T> messageObserver)
        {
            messageObservable.Subscribe<Received<T>>(m => messageObserver.Invoke(m));
        }

        public static void SubscribeReceiver<T>(this IMessageObservable messageObservable, AsyncAction<T> messageObserver)
        {
            messageObservable.Subscribe<Received<T>>((m, t) => messageObserver.Invoke(m, t));
        }
    }
}
