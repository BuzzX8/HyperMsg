using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public interface IMessageBuffer<T>
    {
        void Write(T message);
        Task<FlushResult> FlushAsync(CancellationToken token = default);
    }
}
