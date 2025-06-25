using HyperMsg.Buffers;
using HyperMsg.Transport;
using System.Net.Sockets;

public class SocketTransport : ITransportContext, IConnection, IAsyncDisposable
{
    private readonly Socket _socket;
    private readonly IBufferingContext _bufferingContext;
    private Task? _receiveLoop;
    private CancellationTokenSource? _cts;

    public SocketTransport(Socket socket, IBufferingContext context)
    {
        _socket = socket;
        _bufferingContext = context;

        _bufferingContext.Output.DataWritten += OnDataAppended;
    }

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

    public event Action<Exception> OnError;

    public event Action<ConnectionState> ConnectionStateChanged;

    #endregion

    public IConnection Connection => this;

    public async Task SendAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken)
    {
        if (State != ConnectionState.Connected)
            throw new InvalidOperationException("Cannot send data when the connection is not open.");

        await _socket.SendAsync(data, SocketFlags.None, cancellationToken);
    }

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
                int bytesRead = await _socket.ReceiveAsync(_receiveBuffer, SocketFlags.None, token);
                if (bytesRead == 0)
                {
                    OnDisconnected?.Invoke();
                    break;
                }

                //_context.InputBuffer.Write(_receiveBuffer.AsSpan(0, bytesRead));
            }
        }
        catch (Exception ex)
        {
            OnError?.Invoke(ex);
        }
    }

    private async void OnDataAppended(int _)
    {
        try
        {
            //var data = _context.OutputBuffer.Data;
            //if (data.Length == 0) return;

            //int sent = await _socket.SendAsync(data.ToArray(), SocketFlags.None);
            //_context.OutputBuffer.Advance(sent);
        }
        catch (Exception ex)
        {
            //OnError?.Invoke(ex);
        }
    }

    public event Action<ReadOnlyMemory<byte>> DataReceived;

    public event Action<ReadOnlyMemory<byte>> DataSent;

    public async ValueTask DisposeAsync()
    {
        //_context.OutputBuffer.DataAppended -= OnDataAppended;

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