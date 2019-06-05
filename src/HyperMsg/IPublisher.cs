using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public interface IPublisher
    {
        void Publish<T>(T message);

        Task PublishAsync<T>(T message, CancellationToken cancellationToken);
    }
}
