using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public delegate int BufferReader(ReadOnlySequence<byte> buffer);

    public delegate Task<int> AsyncBufferReader(ReadOnlySequence<byte> buffer, CancellationToken cancellationToken);

    public delegate int BufferSegmentReader(ReadOnlyMemory<byte> buffer);

    public delegate Task<int> AsyncBufferSegmentReader(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken);
}
