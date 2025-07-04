using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace HyperMsg.Transport.Sockets;

internal class SocketConnection(Socket socket, EndPoint endPoint) : IConnection
{
    private readonly Socket _socket = socket ?? throw new ArgumentNullException(nameof(socket), "Socket cannot be null. Please provide a valid socket instance.");
    private readonly EndPoint _endPoint = endPoint ?? throw new ArgumentNullException(nameof(endPoint), "EndPoint cannot be null. Please provide a valid endpoint instance.");

    internal Socket Socket => _socket;

    #region IConnection Members

    /// <summary>
    /// Changes the current connection state and notifies listeners if the state changes.
    /// </summary>
    /// <param name="newState">The new connection state.</param>
    private void ChangeState(ConnectionState newState)
    {
        if (State != newState)
        {
            State = newState;
            StateChanged?.Invoke(State);
        }
    }

    /// <inheritdoc/>
    /// <summary>
    /// Opens the socket connection and starts the receive loop.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <exception cref="InvalidOperationException">Thrown if the connection is already open or the socket is not connected.</exception>
    public async Task OpenAsync(CancellationToken cancellationToken)
    {
        if (State != ConnectionState.Disconnected)
            throw new InvalidOperationException("Connection is already open or opening.");

        ChangeState(ConnectionState.Connecting);

        try
        {
            await _socket.ConnectAsync(_endPoint, cancellationToken);

            ChangeState(ConnectionState.Connected);
        }
        catch (Exception ex)
        {
            ChangeState(ConnectionState.Disconnected);
            OnError?.Invoke(ex);
            throw;
        }
    }

    /// <inheritdoc/>
    /// <summary>
    /// Closes the socket connection and stops the receive loop.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    public Task CloseAsync(CancellationToken cancellationToken)
    {
        if (State == ConnectionState.Disconnected || State == ConnectionState.Disconnecting)
            return Task.CompletedTask;

        ChangeState(ConnectionState.Disconnecting);

        try
        {
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }
        catch (Exception ex)
        {
            OnError?.Invoke(ex);
        }
        finally
        {
            ChangeState(ConnectionState.Disconnected);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public ConnectionState State { get; private set; } = ConnectionState.Disconnected;

    #endregion

    /// <inheritdoc/>
    /// <summary>
    /// Disposes the socket and related resources.
    /// </summary>
    public void Dispose()
    {
        try
        {
            _socket.Shutdown(SocketShutdown.Both);
        }
        catch
        {
            // Ignore exceptions during shutdown, as the socket may already be closed.
            Debugger.Break();
        }

        _socket.Close();
    }

    /// <inheritdoc/>
    public event Action<Exception>? OnError;

    /// <inheritdoc/>
    public event Action<ConnectionState>? StateChanged;
}
