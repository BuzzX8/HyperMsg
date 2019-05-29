using System.Collections.Generic;

namespace HyperMsg
{
    public interface IHandlerRepository
    {
        void AddHandler<T>(IHandler<T> handler);

        IEnumerable<IHandler<T>> GetHandlers<T>();
    }
}
