using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public interface IMessageBuffer<T>
    {
        void Write(T message);
        Task FlushAsync(CancellationToken token = default);
    }
}