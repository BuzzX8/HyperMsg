namespace HyperMsg.Buffers;

/// <summary>
/// Default implementation of IBufferingContext, managing a single Buffer instance.
/// </summary>
public class BufferingContext : IBufferingContext
{
    const ulong DefaultInputBufferSize = 1024 * 1024;
    const ulong DefaultOutputBufferSize = 1024 * 1024;

    private readonly Buffer inputBuffer;
    private readonly Buffer outputBuffer;

    public IBuffer Input => inputBuffer;

    public IBuffer Output => outputBuffer;

    public BufferingContext()
        : this(DefaultInputBufferSize, DefaultOutputBufferSize)
    {
    }

    public BufferingContext(ulong inputBufferSize, ulong outputBufferSize)
    {
        inputBuffer = new (new byte[inputBufferSize]);
        outputBuffer = new (new byte[outputBufferSize]);
    }

    public async ValueTask RequestInputBufferHandling(CancellationToken cancellationToken = default)
    {
        if (InputBufferHandlingRequested != null)
        {
            await InputBufferHandlingRequested(inputBuffer, cancellationToken);
        }
    }

    public async ValueTask RequestOutputBufferHandling(CancellationToken cancellationToken = default)
    {
        if (OutputBufferHandlingRequested != null)
        {
            await OutputBufferHandlingRequested(outputBuffer, cancellationToken);
        }
    }

    public event BufferHandler? InputBufferHandlingRequested;

    public event BufferHandler? OutputBufferHandlingRequested;
}
