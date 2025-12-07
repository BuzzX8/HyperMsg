using System.Net;
using System.Net.Sockets;

namespace HyperMsg.Transport.Sockets;

/// <summary>
/// Adapts a <see cref="Socket"/> instance to the <see cref="ISocket"/> interface,
/// providing asynchronous operations for opening, closing, sending, and receiving data.
/// </summary>
internal class SocketAdapter(Socket socket, EndPoint endPoint) : ISocket
{
    private readonly Socket _socket = socket ?? throw new ArgumentNullException(nameof(socket), "Socket cannot be null. Please provide a valid socket instance.");
    private readonly EndPoint _endPoint = endPoint ?? throw new ArgumentNullException(nameof(endPoint), "EndPoint cannot be null. Please provide a valid endpoint instance.");
    
    /// <summary>
    /// Gets the underlying <see cref="Socket"/> instance.
    /// </summary>
    internal Socket Socket => _socket;

    /// <inheritdoc/>
    /// <summary>
    /// Closes the socket connection asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    public ValueTask CloseAsync(CancellationToken cancellationToken)
    {
        if (_socket.Connected)
        {
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    /// <summary>
    /// Opens the socket connection asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    public ValueTask OpenAsync(CancellationToken cancellationToken)
    {
        if (_socket.Connected)
            throw new InvalidOperationException("Socket is already connected.");

        return _socket.ConnectAsync(_endPoint, cancellationToken);
    }

    /// <inheritdoc/>
    /// <summary>
    /// Receives data asynchronously from the socket connection.
    /// </summary>
    /// <param name="memory">The buffer to store the received data.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    public async ValueTask<int> ReceiveAsync(Memory<byte> memory, CancellationToken cancellationToken)
    {
        if (!_socket.Connected)
            throw new InvalidOperationException("Socket is not connected.");

        var bytesReceived = await _socket.ReceiveAsync(memory, SocketFlags.None, cancellationToken);
        if (bytesReceived > 0)
        {
            OnDataReceived?.Invoke(this, memory[..bytesReceived]);
        }

        return bytesReceived;
    }

    /// <inheritdoc/>
    /// <summary>
    /// Sends data asynchronously over the socket connection.
    /// </summary>
    /// <param name="data">The data to send.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    public async ValueTask<int> SendAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken)
    {
        if (!_socket.Connected)
            throw new InvalidOperationException("Socket is not connected.");

        var bytesSent = await _socket.SendAsync(data, SocketFlags.None, cancellationToken);
        if (bytesSent > 0)
        {
            OnDataSent?.Invoke(this, data[..bytesSent]);
        }

        return bytesSent;
    }

    public event EventHandler<ReadOnlyMemory<byte>>? OnDataSent;

    /// <summary>
    /// Occurs when data is received from the socket.
    /// </summary>
    public event EventHandler<Memory<byte>>? OnDataReceived;
}