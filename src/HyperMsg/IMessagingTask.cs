using System;
using System.Threading.Tasks;

namespace HyperMsg
{
    public interface IMessagingTask : IDisposable
    {
        Task Completion { get; }
    }

    public interface IMessagingTask<T> : IDisposable
    {
        Task<T> Completion { get; }
    }
}
