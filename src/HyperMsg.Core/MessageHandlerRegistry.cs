using System;

namespace HyperMsg
{
    public class MessageHandlerRegistry<T> : IMessageHandlerRegistry<T>
    {
        private Action<T> handlers;

        public void Register(Action<T> handler) => handlers += handler;

        public void Handle(T message)
        {
            handlers?.Invoke(message);
        }
    }
}