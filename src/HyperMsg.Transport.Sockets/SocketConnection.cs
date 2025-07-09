using System.Diagnostics;

namespace HyperMsg.Transport.Sockets;

internal class SocketConnection(ISocket socket) : IConnection, IDisposable
{
    private readonly ISocket _socket = socket ?? throw new ArgumentNullException(nameof(socket), "Socket cannot be null. Please provide a valid socket instance.");

    internal ISocket Socket => _socket;

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
            await _socket.OpenAsync(cancellationToken);

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
            return _socket.CloseAsync(cancellationToken);
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
            ChangeState(ConnectionState.Disconnecting);
            _socket.CloseAsync(default).Wait();
            ChangeState(ConnectionState.Disconnected);
        }
        catch
        {
            // Ignore exceptions during shutdown, as the socket may already be closed.
            Debugger.Break();
        }
    }

    /// <inheritdoc/>
    public event Action<Exception>? OnError;

    /// <inheritdoc/>
    public event Action<ConnectionState>? StateChanged;
}
