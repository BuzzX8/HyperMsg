using System;
using System.Threading.Tasks;

namespace HyperMsg
{
    public interface IHyperTask : IDisposable
    {
        Task Completion { get; }
    }

    public interface IHyperTask<T> : IDisposable
    {
        Task<T> Completion { get; }
    }
}
