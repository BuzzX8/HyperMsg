using System.Net;
using System.Net.Sockets;

namespace HyperMsg.Transport.Sockets;

/// <summary>
/// Provides a socket-based transport implementation for the HyperMsg framework.
/// </summary>
/// <remarks>
/// This class manages the lifecycle and data transmission of a <see cref="Socket"/> connection, including state management and event notification.
/// </remarks>
public class SocketTransport(Socket socket, EndPoint endPoint) : ITransportContext, IDisposable
{
    private readonly SocketConnection _connection = new(socket, endPoint);

    #region ITransportContext Members

    /// <inheritdoc/>
    public IConnection Connection => _connection;

    /// <inheritdoc/>
    public ICollection<ReceiveDataHandler> ReceiveDataHandlers { get; } = [];

    /// <inheritdoc/>
    /// <summary>
    /// Sends data asynchronously over the socket connection.
    /// </summary>
    /// <param name="data">The data to send.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <exception cref="InvalidOperationException">Thrown if the connection is not open.</exception>
    public async Task SendAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken)
    {
        if (_connection.State != ConnectionState.Connected)
            throw new InvalidOperationException("Cannot send data when the connection is not open.");

        await _connection.Socket.SendAsync(data, SocketFlags.None, cancellationToken);
    }

    #endregion

    public void Dispose() => _connection.Dispose();
}