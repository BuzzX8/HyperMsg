using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public interface ICommandHandler
    {
        void Handle<T>(T command);

        Task HandleAsync<T>(T command, CancellationToken token = default);
    }
}