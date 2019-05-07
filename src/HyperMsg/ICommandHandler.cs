using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public interface ICommandHandler<T>
    {
        void Handle(T command);

        Task HandleAsync(T command, CancellationToken token = default);
    }
}