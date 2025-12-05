namespace HyperMsg.Buffers;

/// <summary>
/// Provides context for buffer management, exposing input and output buffers,
/// as well as mechanisms for handling buffer processing through registered handlers.
/// </summary>
public interface IBufferingContext
{
    /// <summary>
    /// Gets the input buffer used for reading data.
    /// </summary>
    IBuffer Input { get; }
    
    /// <summary>
    /// Gets the output buffer used for writing data.
    /// </summary>
    IBuffer Output { get; }

    /// <summary>
    /// Requests processing of the specified input buffer by invoking all registered input handlers.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask RequestInputBufferHandling(CancellationToken cancellationToken);

    /// <summary>
    /// Requests processing of the specified output buffer by invoking all registered output handlers.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask RequestOutputBufferHandling(CancellationToken cancellationToken);

    public event BufferHandler? InputBufferHandlingRequested;

    public event BufferHandler? OutputBufferHandlingRequested;
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