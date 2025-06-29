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
    /// Gets the collection of handlers to process the input buffer.
    /// Each handler is a function that takes an <see cref="IBuffer"/> and returns a <see cref="Task"/>.
    /// </summary>
    ICollection<Func<IBuffer, Task>> InputHandlers { get; }

    /// <summary>
    /// Gets the collection of handlers to process the output buffer.
    /// Each handler is a function that takes an <see cref="IBuffer"/> and returns a <see cref="Task"/>.
    /// </summary>
    ICollection<Func<IBuffer, Task>> OutputHandlers { get; }

    /// <summary>
    /// Requests processing of the specified input buffer by invoking all registered input handlers.
    /// </summary>
    /// <param name="buffer">The input buffer to be processed.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task RequestInputBufferHandling(IBuffer buffer, CancellationToken cancellationToken = default);

    /// <summary>
    /// Requests processing of the specified output buffer by invoking all registered output handlers.
    /// </summary>
    /// <param name="buffer">The output buffer to be processed.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task RequestOutputBufferHandling(IBuffer buffer, CancellationToken cancellationToken = default);
}
