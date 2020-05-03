using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public interface ITask
    {
        Task InitializeAsync(ITaskCompletionSource completionSource, CancellationToken cancellationToken);

        void Dispose();
    }

    public interface ITask<T>
    {
        Task InitializeAsync(ITaskCompletionSource<T> completionSource, CancellationToken cancellationToken);

        void Dispose();
    }
}