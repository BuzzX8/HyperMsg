using HyperMsg.Buffers;
using System.Diagnostics;
using System.Net.Sockets;

namespace HyperMsg.Transport;

/// <summary>
/// Provides a socket-based transport implementation for the HyperMsg framework.
/// </summary>
/// <remarks>
/// This class manages the lifecycle and data transmission of a <see cref="Socket"/> connection, including state management and event notification.
/// </remarks>
public class SocketTransport(Socket socket) : ITransportContext, IConnection, IAsyncDisposable
{
    private readonly Socket _socket = socket;
    private CancellationTokenSource? _cts;
    private Task? _receiveLoop;

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
            ConnectionStateChanged?.Invoke(State);
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
            // If the socket is not connected, connect it (assume it's not connected yet)
            if (!_socket.Connected)
            {
                // The socket must have been created with the correct endpoint
                // If not, user should connect manually before passing in
                // Otherwise, you can add endpoint as a parameter to OpenAsync
                // For now, just check if it's connected
                throw new InvalidOperationException("Socket must be connected before opening.");
            }

            ChangeState(ConnectionState.Connected);

            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _receiveLoop = Task.Run(() => ReceiveLoopAsync(_cts.Token), _cts.Token);
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
    public async Task CloseAsync(CancellationToken cancellationToken)
    {
        if (State == ConnectionState.Disconnected || State == ConnectionState.Disconnecting)
            return Task.CompletedTask;

        ChangeState(ConnectionState.Disconnecting);

        try
        {
            _cts?.Cancel();
            if (_receiveLoop is not null)
                await _receiveLoop;
            _cts?.Dispose();
            _cts = null;
            _receiveLoop = null;

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
    }

    /// <inheritdoc/>
    public ConnectionState State { get; private set; } = ConnectionState.Disconnected;

    #endregion

    #region ITransportContext Members

    /// <inheritdoc/>
    public IConnection Connection => this;

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
        if (State != ConnectionState.Connected)
            throw new InvalidOperationException("Cannot send data when the connection is not open.");

        await _socket.SendAsync(data, SocketFlags.None, cancellationToken);
    }

    #endregion

    /// <summary>
    /// Asynchronous loop that receives data from the socket and dispatches it to registered handlers.
    /// </summary>
    /// <param name="token">A token to monitor for cancellation requests.</param>
    private async Task ReceiveLoopAsync(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                //int bytesRead = await _socket.ReceiveAsync(_receiveBuffer, SocketFlags.None, token);
                //if (bytesRead == 0)
                //{
                //    OnDisconnected?.Invoke();
                //    break;
                //}

                //_context.InputBuffer.Write(_receiveBuffer.AsSpan(0, bytesRead));
            }
        }
        catch (Exception ex)
        {
            OnError?.Invoke(ex);
            throw;
        }
    }

    /// <summary>
    /// Handles sending data from the input buffer over the socket.
    /// </summary>
    /// <param name="buffer">The buffer containing data to send.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    internal async Task HandleInputBuffer(IBuffer buffer, CancellationToken cancellationToken)
    {
        try
        {
            var memory = buffer.Reader.GetMemory();

            if (memory.Length == 0) return;

            //int sent = await _socket.SendAsync(data.ToArray(), SocketFlags.None);
            //_context.OutputBuffer.Advance(sent);
        }
        catch (Exception ex)
        {
            OnError?.Invoke(ex);
        }
    }

    /// <summary>
    /// Handles sending data from the output buffer over the socket.
    /// </summary>
    /// <param name="buffer">The buffer containing data to send.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    internal async Task HandleOutputBuffer(IBuffer buffer, CancellationToken cancellationToken)
    {
        try
        {
            var memory = buffer.Reader.GetMemory();

            if (memory.Length == 0) return;

            int sent = await _socket.SendAsync(memory, SocketFlags.None, cancellationToken);
            buffer.Reader.Advance(sent);
        }
        catch (Exception ex)
        {
            OnError?.Invoke(ex);
        }
    }

    /// <inheritdoc/>
    /// <summary>
    /// Disposes the socket and related resources asynchronously.
    /// </summary>
    public async ValueTask DisposeAsync()
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

        //if (_cts is not null)
        //{
        //    _cts.Cancel();
        //    if (_receiveLoop is not null)
        //        await _receiveLoop;
        //    _cts.Dispose();
        //}
    }

    /// <inheritdoc/>
    public event Action<ConnectionState> ConnectionStateChanged;

    /// <summary>
    /// Occurs when data is received from the socket.
    /// </summary>
    public event Action<ReadOnlyMemory<byte>> DataReceived;

    /// <inheritdoc/>
    public event Action<Exception> OnError;

}