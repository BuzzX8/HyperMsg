using System.Buffers;

namespace HyperMsg
{
    internal struct SerializationCommand<T>
    {
        internal SerializationCommand(IBufferWriter<byte> bufferWriter, T message)
        {
            BufferWriter = bufferWriter;
            Message = message;
        }

        public IBufferWriter<byte> BufferWriter { get; }

        public T Message { get; }
    }
}
