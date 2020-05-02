using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public interface ITask : IDisposable
    {
        Task StartAsync(ITaskCompletionSource completionSource, CancellationToken cancellationToken);
    }

    public interface ITask<T> : IDisposable
    {
        Task StartAsync(ITaskCompletionSource<T> completionSource, CancellationToken cancellationToken);
    }
}
