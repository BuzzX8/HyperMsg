using System.Buffers;

namespace HyperMsg.Messages
{
    internal struct SerializeCommand<T>
    {
        internal SerializeCommand(IBufferWriter<byte> bufferWriter, T message)
        {
            BufferWriter = bufferWriter;
            Message = message;
        }

        public IBufferWriter<byte> BufferWriter { get; }

        public T Message { get; }
    }
}
