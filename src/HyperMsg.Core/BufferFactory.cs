using System.Buffers;

namespace HyperMsg;

public class BufferFactory
{
    private readonly MemoryPool<byte> memoryPool;

    public static readonly BufferFactory Shared = new(MemoryPool<byte>.Shared);

    private BufferFactory(MemoryPool<byte> memoryPool)
    {
        this.memoryPool = memoryPool ?? throw new ArgumentNullException(nameof(memoryPool));
    }

    public Buffer CreateBuffer(int bufferSize)
    {
        var memory = memoryPool.Rent(bufferSize);
        return new Buffer(memory);
    }
}
