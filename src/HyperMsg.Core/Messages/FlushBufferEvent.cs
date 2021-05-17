using System;

namespace HyperMsg.Messages
{
    internal struct FlushBufferEvent
    {
        public FlushBufferEvent(BufferType bufferType, Action<BufferReader> bufferReaderAction, Action<AsyncBufferReader> asyncBufferReaderAction) => 
            (BufferType, BufferReaderAction, AsyncBufferReaderAction) = (bufferType, bufferReaderAction, asyncBufferReaderAction);

        public BufferType BufferType { get; }

        public Action<BufferReader> BufferReaderAction { get; }

        public Action<AsyncBufferReader> AsyncBufferReaderAction { get; }
    }
}