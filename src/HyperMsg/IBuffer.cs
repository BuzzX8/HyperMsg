using System.Buffers;

namespace HyperMsg
{
    public interface IBuffer
    {
        IBufferWriter<byte> Writer { get; }

        event AsyncAction<IBufferReader> DataCommitted;
    }

    public interface ISendingBuffer : IBuffer
    { }

    public interface IReceivingBuffer : IBuffer
    { }
}