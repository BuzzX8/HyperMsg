namespace HyperMsg.Buffers;

/// <summary>
/// Default implementation of IBufferingContext, managing a single Buffer instance.
/// </summary>
public class BufferingContext(ulong inputBufferSize, ulong outputBufferSize) : IBufferingContext
{
    private readonly Buffer inputBuffer = new(new byte[inputBufferSize]);
    private readonly Buffer outputBuffer = new(new byte[outputBufferSize]);

    public IBuffer Input => inputBuffer;
    public IBuffer Output => outputBuffer;

    public ICollection<BufferHandler> InputHandlers { get; } = [];

    public ICollection<BufferHandler> OutputHandlers { get; } = [];

    public async Task RequestInputBufferHandling(CancellationToken cancellationToken = default)
    {
        // Invoke all input handlers with the provided buffer
        foreach (var handler in InputHandlers)
        {
            await handler.Invoke(inputBuffer, cancellationToken);
        }
    }

    public async Task RequestOutputBufferHandling(CancellationToken cancellationToken = default)
    {
        // Invoke all output handlers with the provided buffer
        foreach (var handler in OutputHandlers)
        {
            await handler.Invoke(outputBuffer, cancellationToken);
        }
    }
}
