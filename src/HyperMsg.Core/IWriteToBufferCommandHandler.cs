using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public interface IWriteToBufferCommandHandler
    {
        public void Handle<T>(T message, BufferType bufferType);

        public Task HandleAsync<T>(T message, BufferType bufferType, CancellationToken cancellationToken);
    }

    public enum BufferType
    {
        None,
        Transmitting,
        Receiving
    }
}
