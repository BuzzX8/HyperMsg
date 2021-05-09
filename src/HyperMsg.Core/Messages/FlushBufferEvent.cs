using System;

namespace HyperMsg.Messages
{
    internal struct FlushBufferEvent
    {
        public FlushBufferEvent(BufferType bufferType, Action<BufferReader> bufferReaderAction) => (BufferType, BufferReaderAction) = (bufferType, bufferReaderAction);

        public BufferType BufferType { get; }

        public Action<BufferReader> BufferReaderAction { get; }
    }
}