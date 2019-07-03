using System;

namespace HyperMsg
{
    public interface IMessageHandlerRegistry<T>
    {
        void Register(Action<T> handler);
    }
}