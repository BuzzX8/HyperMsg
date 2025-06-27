
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

    public ICollection<Func<IBuffer, Task>> InputHandlers => throw new NotImplementedException();

    public ICollection<Func<IBuffer, Task>> OutputHandlers => throw new NotImplementedException();

    public Task RequestInputBufferHandling(IBuffer buffer, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task RequestOutputBufferHandling(IBuffer buffer, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
