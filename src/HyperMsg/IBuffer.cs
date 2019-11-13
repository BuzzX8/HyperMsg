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

        event AsyncAction<IBufferReader<byte>> FlushRequested;
    }

    public interface ITransmittingBuffer : IBuffer
    { }

    public interface IReceivingBuffer : IBuffer
    { }
}