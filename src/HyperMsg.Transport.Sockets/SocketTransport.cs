using System.Net;
using System.Net.Sockets;

namespace HyperMsg.Transport.Sockets;

/// <summary>
/// Provides a socket-based transport implementation for the HyperMsg framework.
/// </summary>
/// <remarks>
/// This class manages the lifecycle and data transmission of a <see cref="Socket"/> connection, including state management and event notification.
/// </remarks>
public class SocketTransport(ISocket socket) : ITransportContext, IDisposable
{
    private readonly SocketConnection _connection = new(socket);
    private readonly SocketChannel _channel = new(socket);

    #region ITransportContext Members

    /// <inheritdoc/>
    public IConnection Connection => _connection;

    public IChannel Channel => _channel;

    #endregion

    public void Dispose()
    {
        _connection.Dispose();
    }
}