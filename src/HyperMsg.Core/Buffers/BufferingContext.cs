namespace HyperMsg.Buffers;

/// <summary>
/// Default implementation of IBufferingContext, managing a single Buffer instance.
/// </summary>
public class BufferingContext(Memory<byte> memory) : IBufferingContext
{
    private readonly Buffer inputBuffer = new(memory);
    private readonly Buffer outputBuffer = new(memory);

    public IBuffer Input => inputBuffer;
    public IBuffer Output => outputBuffer;
}
