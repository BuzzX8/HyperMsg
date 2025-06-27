namespace HyperMsg.Transport;

/// <summary>
/// Represents a transport context that manages the connection and data transmission for a transport layer.
/// </summary>
/// <remarks>
/// Implementations of this interface are responsible for handling the underlying connection and providing asynchronous data sending capabilities.
/// </remarks>
public interface ITransportContext : IAsyncDisposable
{
    /// <summary>
    /// Gets the connection associated with the transport context.
    /// </summary>
    IConnection Connection { get; }
    
    /// <summary>
    /// Asynchronously sends data over the transport connection.
    /// </summary>
    /// <param name="data">The data to send as a read-only memory buffer.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous send operation.</returns>
    Task SendAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default);

    /// <summary>
    /// Occurs when data is received from the transport connection.
    /// </summary>
    event Action<ReadOnlyMemory<byte>> DataReceived;
}