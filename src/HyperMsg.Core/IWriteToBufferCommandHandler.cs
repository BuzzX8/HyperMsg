using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public interface IWriteToBufferCommandHandler
    {
        public void WriteToBuffer<T>(BufferType bufferType, T message);

        public Task WriteToBufferAsync<T>(BufferType bufferType, T message, CancellationToken cancellationToken);
    }

    public enum BufferType
    {
        None,
        Transmitting,
        Receiving
    }
}
