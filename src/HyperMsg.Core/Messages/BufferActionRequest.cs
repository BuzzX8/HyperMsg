using System;

namespace HyperMsg.Messages
{
    internal struct BufferActionRequest
    {
        public BufferActionRequest(BufferType bufferType, Action<IBuffer> bufferAction)
        {
            BufferAction = bufferAction;
            BufferType = bufferType;
        }

        public Action<IBuffer> BufferAction { get; }

        public BufferType BufferType { get; }
    }
}
