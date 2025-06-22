using HyperMsg.Buffers;
using HyperMsg.Transport;
using System.Net.Sockets;

public class SocketTransport : ITransport
{
    private readonly Socket _socket;
    private readonly IBufferingContext _context;
    private readonly byte[] _receiveBuffer;

    private Task? _receiveLoop;
    private CancellationTokenSource? _cts;

    public event Action<Exception>? OnError;
    public event Action? OnDisconnected;

    public SocketTransport(Socket socket, IBufferingContext context, int bufferSize = 1024)
    {
        _socket = socket;
        _context = context;
        _receiveBuffer = new byte[bufferSize];

        //_context.OutputBuffer.DataAppended += OnDataAppended;
    }

    public Task StartAsync(CancellationToken cancellationToken)
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
            OnError?.Invoke(ex);
        }
    }

    public async Task SendAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken)
    {
        await _socket.SendAsync(data, SocketFlags.None, cancellationToken);
    }

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