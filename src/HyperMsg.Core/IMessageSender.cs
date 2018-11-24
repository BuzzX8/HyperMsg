using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public interface IMessageSender<T>
    {
        Task SendAsync(T message, CancellationToken token = default);
    }
}
