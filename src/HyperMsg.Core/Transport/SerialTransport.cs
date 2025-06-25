using System.IO.Ports;

namespace HyperMsg.Transport;

public class SerialTransport(SerialPort serialPort) : ITransportContext, IConnection, IAsyncDisposable
{
    private readonly SerialPort serialPort = serialPort;

    public IConnection Connection => throw new NotImplementedException();

    public Stream InputStream => throw new NotImplementedException();

    public Stream OutputStream => throw new NotImplementedException();

    public ConnectionState State => throw new NotImplementedException();

    public Task SendAsync(string message, CancellationToken cancellationToken = default)
    {
        // Implementation for sending a message over serial transport
        throw new NotImplementedException();
    }

    public Task<string> ReceiveAsync(CancellationToken cancellationToken = default)
    {
        // Implementation for receiving a message over serial transport
        throw new NotImplementedException();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (serialPort.IsOpen)
        {
            return Task.CompletedTask; // Already started
        }

        try
        {
            serialPort.Open();
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            OnError?.Invoke(ex);
            return Task.FromException(ex);
        }
    }

    public Task SendAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken)
    {
        if (!serialPort.IsOpen)
        {
            throw new InvalidOperationException("Serial port is not open.");
        }
        try
        {
            //serialPort.Write(data.Span);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            OnError?.Invoke(ex);
            return Task.FromException(ex);
        }
    }

    ValueTask IAsyncDisposable.DisposeAsync()
    {
        serialPort?.Dispose();
        return ValueTask.CompletedTask;
    }

    public Task OpenAsync(CancellationToken cancellationToken)
    {
        if (serialPort.IsOpen)
        {
            return Task.CompletedTask; // Already open
        }

        try
        {
            serialPort.Open();
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            OnError?.Invoke(ex);
            return Task.FromException(ex);
        }
    }

    public Task CloseAsync(CancellationToken cancellationToken)
    {
        if (!serialPort.IsOpen)
        {
            return Task.CompletedTask; // Already closed
        }

        try
        {
            serialPort.Close();
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            OnError?.Invoke(ex);
            return Task.FromException(ex);
        }
    }

    public event Action<Exception> OnError;
    public event Action OnDisconnected;
    public event Action<int> DataReceived;
    public event Action<int> DataSent;
    public event Action<ConnectionState> ConnectionStateChanged;
}