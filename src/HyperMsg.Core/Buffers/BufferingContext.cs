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

    public ICollection<Func<IBuffer, Task>> InputHandlers { get; } = [];

    public ICollection<Func<IBuffer, Task>> OutputHandlers { get; } = [];

    public async Task RequestInputBufferHandling(IBuffer buffer, CancellationToken cancellationToken = default)
    {
        if (buffer is null)
        {
            throw new ArgumentNullException(nameof(buffer), "Buffer cannot be null.");
        }
        // Invoke all input handlers with the provided buffer
        foreach (var handler in InputHandlers)
        {
            await handler.Invoke(buffer);
        }
    }

    public async Task RequestOutputBufferHandling(IBuffer buffer, CancellationToken cancellationToken = default)
    {
        if (buffer is null)
        {
            throw new ArgumentNullException(nameof(buffer), "Buffer cannot be null.");
        }
        // Invoke all output handlers with the provided buffer
        foreach (var handler in OutputHandlers)
        {
            await handler.Invoke(buffer);
        }
    }
}
