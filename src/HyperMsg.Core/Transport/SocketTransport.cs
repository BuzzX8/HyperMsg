using HyperMsg.Buffers;
using HyperMsg.Transport;
using System.Net.Sockets;

public class SocketTransport : ITransportContext, IConnection, IAsyncDisposable
{
    private readonly Socket _socket;
    private readonly IBuffer _outputBuffer;
    private Task? _receiveLoop;
    private CancellationTokenSource? _cts;

    public SocketTransport(Socket socket, IBuffer outputBuffer)
    {
        _socket = socket;
        _outputBuffer = outputBuffer;

        _outputBuffer.DataAppended += OnDataAppended;
    }

    #region IConnection Members

    public Task OpenAsync(CancellationToken cancellationToken)
    {
        if (State == ConnectionState.Connected)
            return Task.CompletedTask;

        if (State == ConnectionState.Connecting)
            throw new InvalidOperationException("Connection is already in progress.");

        State = ConnectionState.Connecting;

        try
        {
            _socket.Connect(_socket.RemoteEndPoint!);
            State = ConnectionState.Connected;
            StartAsync(cancellationToken);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            State = ConnectionState.Disconnected;
            OnError?.Invoke(ex);
            throw;
        }
        
    }

    public Task CloseAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public ConnectionState State { get; private set; } = ConnectionState.Disconnected;

    public event Action<Exception> OnError;

    public event Action<ConnectionState> ConnectionStateChanged;

    #endregion

    #region ITransportContext Members
    
    public IConnection Connection => this;

    public async Task SendAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken)
    {
        if (State != ConnectionState.Connected)
            throw new InvalidOperationException("Cannot send data when the connection is not open.");

        await _socket.SendAsync(data, SocketFlags.None, cancellationToken);
    }

    #endregion

    private Task StartAsync(CancellationToken cancellationToken)
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _receiveLoop = Task.Run(() => ReceiveLoopAsync(_cts.Token), _cts.Token);
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
            OnError?.Invoke(ex);
        }
    }

    private void OnDataAppended(int bytesAppended)
    {
        try
        {
            var memory = _outputBuffer.Reader.GetMemory();

            if (memory.Length == 0) return;

            //int sent = await _socket.SendAsync(data.ToArray(), SocketFlags.None);
            //_context.OutputBuffer.Advance(sent);
        }
        catch (Exception ex)
        {
            //OnError?.Invoke(ex);
        }
    }

    public event Action<ReadOnlyMemory<byte>> DataReceived;

    public async ValueTask DisposeAsync()
    {
        _outputBuffer.DataAppended -= OnDataAppended;

        //try { _socket.Shutdown(SocketShutdown.Both); } catch { }
        //_socket.Close();

        //if (_cts is not null)
        //{
        //    _cts.Cancel();
        //    if (_receiveLoop is not null)
        //        await _receiveLoop;
        //    _cts.Dispose();
        //}
    }
}