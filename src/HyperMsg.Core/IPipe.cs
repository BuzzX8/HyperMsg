using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public interface IPipe
    {
        IPipeReader Reader { get; }

        IPipeWriter Writer { get; }
    }

    public interface IPipeReader
    {
        ReadOnlySequence<byte> Read();

        Task<ReadOnlySequence<byte>> ReadAsync(CancellationToken token = default);

        void Advance(int length);
    }

    public interface IPipeWriter : IBufferWriter<byte>
    {
        void Flush();

        Task FlushAsync(CancellationToken token = default);
    }
}
