namespace HyperMsg.Transport.Sockets;

public interface ISocket
{
    /// <summary>
    /// Opens the socket connection asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    Task OpenAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Closes the socket connection asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    Task CloseAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Sends data asynchronously over the socket connection.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task SendAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken);

    Task ReceiveAsync(Memory<byte> memory, CancellationToken cancellationToken);

    event EventHandler<Memory<byte>> OnDataReceived;
}
