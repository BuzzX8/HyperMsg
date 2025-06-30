using HyperMsg.Buffers;
using System.Diagnostics;
using System.Net.Sockets;

namespace HyperMsg.Transport;

public class SocketTransport(Socket socket) : ITransportContext, IConnection, IAsyncDisposable
{
    private readonly Socket _socket = socket;

    #region IConnection Members

    public Task OpenAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();        
    }

    public Task CloseAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public ConnectionState State { get; private set; } = ConnectionState.Disconnected;

    #endregion

    #region ITransportContext Members

    public IConnection Connection => this;

    public ICollection<ReceiveDataHandler> ReceiveDataHandlers { get; } = [];

    public async Task SendAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken)
    {
        if (State != ConnectionState.Connected)
            throw new InvalidOperationException("Cannot send data when the connection is not open.");

        await _socket.SendAsync(data, SocketFlags.None, cancellationToken);
    }

    #endregion

    private Task StartAsync(CancellationToken cancellationToken)
    {
        //_cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        //_receiveLoop = Task.Run(() => ReceiveLoopAsync(_cts.Token), _cts.Token);
        return Task.CompletedTask;
    }

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
            //OnError?.Invoke(ex);
        }
    }

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

    public event Action<ConnectionState> ConnectionStateChanged;

    public event Action<ReadOnlyMemory<byte>> DataReceived;

    public event Action<Exception> OnError;

}