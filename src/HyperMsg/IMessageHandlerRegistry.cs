using System;

namespace HyperMsg
{
    public interface IMessageHandlerRegistry
    {
        void Register<T>(Action<T> handler);

        void Register<T>(AsyncAction<T> handler);
    }
}