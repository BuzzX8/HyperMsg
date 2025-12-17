namespace HyperMsg.Buffers;

public class BufferingContext : IBufferingContext
{
    const ulong DefaultInputBufferSize = 1024 * 1024;
    const ulong DefaultOutputBufferSize = 1024 * 1024;

    private readonly Buffer inputBuffer;
    private readonly Buffer outputBuffer;

    public IBuffer InputBuffer => inputBuffer;

    public IBuffer OutputBuffer => outputBuffer;

    public BufferingContext()
        : this(DefaultInputBufferSize, DefaultOutputBufferSize)
    {
    }

    public BufferingContext(ulong inputBufferSize, ulong outputBufferSize)
    {
        inputBuffer = new (new byte[inputBufferSize]);
        outputBuffer = new (new byte[outputBufferSize]);
    }


    public async ValueTask RequestInputBufferDownstreamUpdate(CancellationToken cancellationToken = default)
    {
        if (InputBufferDownstreamUpdateRequested != null)
        {
            await InputBufferDownstreamUpdateRequested(inputBuffer, cancellationToken);
        }
    }

    public async ValueTask RequestInputBufferUpstreamUpdate(CancellationToken cancellationToken = default)
    {
        if (InputBufferUpstreamUpdateRequested != null)
        {
            await InputBufferUpstreamUpdateRequested(inputBuffer, cancellationToken);
        }
    }

    public async ValueTask RequestOutputBufferDownstreamUpdate(CancellationToken cancellationToken = default)
    {
        if (OutputBufferDownstreamUpdateRequested != null)
        {
            await OutputBufferDownstreamUpdateRequested(outputBuffer, cancellationToken);
        }
    }

    public async ValueTask RequestOutputBufferUpstreamUpdate(CancellationToken cancellationToken = default)
    {
        if (OutputBufferUpstreamUpdateRequested != null)
        {
            await OutputBufferUpstreamUpdateRequested(outputBuffer, cancellationToken);
        }
    }

    public event BufferHandler? InputBufferDownstreamUpdateRequested;

    public event BufferHandler? InputBufferUpstreamUpdateRequested;

    public event BufferHandler? OutputBufferDownstreamUpdateRequested;

    public event BufferHandler? OutputBufferUpstreamUpdateRequested;
}
