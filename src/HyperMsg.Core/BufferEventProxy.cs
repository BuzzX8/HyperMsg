using System;
using System.Buffers;

namespace HyperMsg
{
    internal class BufferEventProxy : IBuffer, IBufferWriter
    {
        private readonly BufferType bufferType;
        private readonly IBuffer buffer;
        private readonly ISender sender;

        public IBufferReader Reader => buffer.Reader;

        public IBufferWriter Writer => this;

        public void Clear() => buffer.Clear();

        void IBufferWriter<byte>.Advance(int count)
        {
            buffer.Writer.Advance(count);
            sender.Send(new BufferUpdatedEvent());
        }

        Memory<byte> IBufferWriter<byte>.GetMemory(int sizeHint) => buffer.Writer.GetMemory(sizeHint);

        Span<byte> IBufferWriter<byte>.GetSpan(int sizeHint) => buffer.Writer.GetSpan(sizeHint);
    }

    public struct BufferUpdatedEvent
    {
        public BufferUpdatedEvent(BufferType bufferType, IBufferReader bufferReader)
            => (BufferType, BufferReader) = (bufferType, bufferReader);

        public BufferType BufferType {get;}

        public IBufferReader BufferReader {get;}
    }
}