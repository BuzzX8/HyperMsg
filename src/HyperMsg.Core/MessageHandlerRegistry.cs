using System;

namespace HyperMsg
{
    public class MessageHandlerRegistry<T> : IMessageHandlerRegistry<T>
    {
        private Action<T> handlers;
        private AsyncAction<T> asyncHandlers;

        public void Register(Action<T> handler) => handlers += handler;

        public void Register(AsyncAction<T> handler) => asyncHandlers += handler;

        public void Handle(T message)
        {
            handlers?.Invoke(message);
        }
    }
}