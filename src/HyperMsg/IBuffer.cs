using System.Buffers;

namespace HyperMsg
{
    public interface IBuffer
    {
        IBufferReader Reader { get; }

        IBufferWriter<byte> Writer { get; }

        event AsyncAction<IBuffer> Flushed;
    }

    public interface ISendingBuffer : IBuffer
    { }

    public interface IReceivingBuffer : IBuffer
    { }
}