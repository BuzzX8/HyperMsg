using System.Buffers;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public interface IBuffer
    {
        IBufferReader<byte> Reader { get; }

        IBufferWriter<byte> Writer { get; }

        void Clear();

        Task FlushAsync(CancellationToken cancellationToken);

        event AsyncAction<IBuffer> FlushRequested;
    }

    public interface ISendingBuffer : IBuffer
    { }

    public interface IReceivingBuffer : IBuffer
    { }
}