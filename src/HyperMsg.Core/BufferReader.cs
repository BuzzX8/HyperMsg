using System.Buffers;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public delegate int BufferReader(ReadOnlySequence<byte> buffer);

    public delegate Task<int> AsyncBufferReader(ReadOnlySequence<byte> buffer, CancellationToken cancellationToken);
}
