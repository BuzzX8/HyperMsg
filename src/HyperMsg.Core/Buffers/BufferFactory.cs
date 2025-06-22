using System.Buffers;

namespace HyperMsg.Buffers;

public static class BufferFactory
{
    public static IBufferingContext NewContext(IMemoryOwner<byte> memoryOwner)
    {
        // Create a new instance of BufferingContext
        return new BufferingContext(memoryOwner.Memory);
    }

    public static IBufferingContext NewContext(Memory<byte> memory)
    {
        // Create a new instance of BufferingContext with the provided Memory<byte>
        return new BufferingContext(memory);
    }

    public static IBuffer NewBuffer(IMemoryOwner<byte> memoryOwner)
    {
        // Create a new Buffer instance using the provided IMemoryOwner<byte>
        return new Buffer(memoryOwner.Memory);
    }

    public static IBuffer NewBuffer(Memory<byte> memory)
    {
        // Create a new Buffer instance using the provided Memory<byte>
        return new Buffer(memory);
    }
}
