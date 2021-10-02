using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    internal class BufferEventProxy : IBuffer
    {
        private readonly BufferType bufferType;
        private readonly Buffer buffer;
        private readonly ISender sender;

        public BufferEventProxy(BufferType bufferType, Buffer buffer, ISender sender) =>
            (this.bufferType, this.buffer, this.sender) = (bufferType, buffer, sender);

        public IBufferReader Reader => buffer.Reader;

        public IBufferWriter Writer => buffer.Writer;

        public void Clear() => buffer.Clear();

        public void Flush() => sender.Send(new BufferUpdatedEvent(bufferType, buffer.Reader));

        public Task FlushAsync(CancellationToken cancellationToken) => sender.SendAsync(new BufferUpdatedEvent(bufferType, buffer.Reader), cancellationToken);

        public void Dispose() => buffer.Dispose();
    }

    public struct BufferUpdatedEvent
    {
        public BufferUpdatedEvent(BufferType bufferType, IBufferReader bufferReader)
            => (BufferType, BufferReader) = (bufferType, bufferReader);

        public BufferType BufferType {get;}

        public IBufferReader BufferReader {get;}
    }

    public enum BufferType
    {
        Receive,
        Transmit
    }
}