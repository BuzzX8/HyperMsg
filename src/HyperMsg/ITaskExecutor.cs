using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public interface ITaskExecutor
    {
        Task ExecuteAsync(ITask task, CancellationToken cancellationToken);

        Task<T> ExecuteAsync<T>(ITask<T> task, CancellationToken cancellationToken);
    }
}