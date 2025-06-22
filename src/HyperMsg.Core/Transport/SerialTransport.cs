using System.IO.Ports;

namespace HyperMsg.Transport;

public class SerialTransport(SerialPort serialPort) : ITransport
{
    private readonly SerialPort serialPort = serialPort;

    public string Name => "Serial";

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

    public event Action<Exception> OnError;
    public event Action OnDisconnected;
}