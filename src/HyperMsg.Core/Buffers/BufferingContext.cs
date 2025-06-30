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

    public async Task RequestInputBufferHandling(IBuffer buffer, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(buffer, nameof(buffer));

        // Invoke all input handlers with the provided buffer
        foreach (var handler in InputHandlers)
        {
            await handler.Invoke(buffer, cancellationToken);
        }
    }

    public async Task RequestOutputBufferHandling(IBuffer buffer, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(buffer, nameof(buffer));

        // Invoke all output handlers with the provided buffer
        foreach (var handler in OutputHandlers)
        {
            await handler.Invoke(buffer, cancellationToken);
        }
    }
}
