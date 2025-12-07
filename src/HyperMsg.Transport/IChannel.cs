namespace HyperMsg.Transport;

/// <summary>
/// Represents a bidirectional byte channel used for sending and receiving raw bytes.
/// </summary>
/// <remarks>
/// Implementations should avoid retaining references to caller-owned buffers. Methods return the number of bytes
/// successfully sent or read. Cancellation requests should cause the operation to complete promptly and typically
/// result in an <see cref="OperationCanceledException"/>.
/// </remarks>
public interface IChannel
{
    /// <summary>
    /// Sends the specified bytes asynchronously over the channel.
    /// </summary>
    /// <param name="data">The bytes to send. The callee must not modify the contents of this buffer.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the send to complete. If cancellation is requested the operation should complete promptly and typically throw <see cref="OperationCanceledException"/>.</param>
    /// <returns>
    /// A <see cref="ValueTask{Int32}"/> that completes with the number of bytes written.
    /// </returns>
    /// <example>
    /// <code>
    /// var sent = await channel.SendAsync(message, cancellationToken);
    /// </code>
    /// </example>
    ValueTask<int> SendAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default);

    /// <summary>
    /// Receives data into the provided buffer asynchronously.
    /// </summary>
    /// <param name="data">A buffer to receive bytes into. On completion the first <c>n</c> bytes of the buffer contain the received data, where <c>n</c> is the returned value.</param>
    /// <param name="cancellationToken">A token to cancel the receive operation. If cancellation is requested the operation should complete promptly and typically throw <see cref="OperationCanceledException"/>.</param>
    /// <returns>
    /// A <see cref="ValueTask{Int32}"/> that completes with the number of bytes read. A return value of zero typically indicates end-of-stream.
    /// </returns>
    /// <remarks>
    /// Partial reads are allowed; the returned count indicates how many bytes were written into <paramref name="data"/>.
    /// </remarks>
    ValueTask<int> ReceiveAsync(Memory<byte> data, CancellationToken cancellationToken = default);
}
