using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public interface IMessageWaiter<T>
    {
        Task<T> WaitAsync(CancellationToken cancellationToken);

        void SetMessage(T message);
    }
}
