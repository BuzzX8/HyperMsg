using System.Buffers;

namespace HyperMsg
{
    public delegate void SerializeAction<T>(IBufferWriter<byte> writer, T message);
}