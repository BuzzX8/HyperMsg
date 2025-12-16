namespace HyperMsg.Buffers;

/// <summary>
/// Provides context for buffer management, exposing input and output buffers,
/// as well as mechanisms for handling buffer processing through registered handlers.
/// </summary>
public interface IBufferingContext
{
    /// <summary>
    /// Gets the input buffer used for receiving data.
    /// </summary>
    IBuffer InputBuffer { get; }
    
    /// <summary>
    /// Gets the output buffer used for sending data.
    /// </summary>
    IBuffer OutputBuffer { get; }

    /// <summary>
    /// Requests that the input buffer be updated by downstream processors.
    /// </summary>
    /// <param name="cancellationToken">A token to observe while waiting for the update to complete.</param>
    /// <returns>A <see cref="ValueTask"/> that completes when the request has been processed.</returns>
    ValueTask RequestInputBufferDownstreamUpdate(CancellationToken cancellationToken);

    /// <summary>
    /// Requests that the input buffer be updated by upstream processors.
    /// </summary>
    /// <param name="cancellationToken">A token to observe while waiting for the update to complete.</param>
    /// <returns>A <see cref="ValueTask"/> that completes when the request has been processed.</returns>
    ValueTask RequestInputBufferUpstreamUpdate(CancellationToken cancellationToken);

    /// <summary>
    /// Requests that the output buffer be updated by downstream processors.
    /// </summary>
    /// <param name="cancellationToken">A token to observe while waiting for the update to complete.</param>
    /// <returns>A <see cref="ValueTask"/> that completes when the request has been processed.</returns>
    ValueTask RequestOutputBufferDownstreamUpdate(CancellationToken cancellationToken);

    /// <summary>
    /// Requests that the output buffer be updated by upstream processors.
    /// </summary>
    /// <param name="cancellationToken">A token to observe while waiting for the update to complete.</param>
    /// <returns>A <see cref="ValueTask"/> that completes when the request has been processed.</returns>
    ValueTask RequestOutputBufferUpstreamUpdate(CancellationToken cancellationToken);

    /// <summary>
    /// Raised when a downstream update of the input buffer is requested.
    /// Handlers should process the <see cref="InputBuffer"/> and respect the provided <see cref="CancellationToken"/>.
    /// </summary>
    public event BufferHandler? InputBufferDownstreamUpdateRequested;

    /// <summary>
    /// Raised when an upstream update of the input buffer is requested.
    /// Handlers should process the <see cref="InputBuffer"/> and respect the provided <see cref="CancellationToken"/>.
    /// </summary>
    public event BufferHandler? InputBufferUpstreamUpdateRequested;

    /// <summary>
    /// Raised when a downstream update of the output buffer is requested.
    /// Handlers should process the <see cref="OutputBuffer"/> and respect the provided <see cref="CancellationToken"/>.
    /// </summary>
    public event BufferHandler? OutputBufferDownstreamUpdateRequested;

    /// <summary>
    /// Raised when an upstream update of the output buffer is requested.
    /// Handlers should process the <see cref="OutputBuffer"/> and respect the provided <see cref="CancellationToken"/>.
    /// </summary>
    public event BufferHandler? OutputBufferUpstreamUpdateRequested;
}

/// <summary>
/// Represents a delegate that processes a buffer asynchronously.
/// </summary>
/// <param name="buffer">The buffer to be processed. Must not be null.</param>
/// <param name="cancellationToken">A token that can be used to cancel the operation. The implementation should honor the cancellation request if
/// possible.</param>
/// <returns>A <see cref="ValueTask"/> that represents the asynchronous operation. The task completes when the buffer processing
/// is finished.</returns>
public delegate ValueTask BufferHandler(IBuffer buffer, CancellationToken cancellationToken);