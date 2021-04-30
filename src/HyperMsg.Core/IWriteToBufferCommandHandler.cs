using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public interface IWriteToBufferCommandHandler
    {
        public void WriteToBuffer<T>(T message, BufferType bufferType);

        public Task WriteToBufferAsync<T>(T message, BufferType bufferType, CancellationToken cancellationToken);
    }

    public enum BufferType
    {
        None,
        Transmitting,
        Receiving
    }
}
