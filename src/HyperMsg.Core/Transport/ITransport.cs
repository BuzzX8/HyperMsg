namespace HyperMsg.Transport;

public interface ITransport : IAsyncDisposable
{
    IConnection Connection { get; }
    Stream InputStream { get; }
    Stream OutputStream { get; }

    // Event triggered when data is received on the input stream
    event Action<int> DataReceived;

    // Event triggered when data is written to the output stream
    event Action<int> DataSent;

    // Event triggered when the connection state changes (e.g., connected/disconnected)
    event Action<ConnectionState> ConnectionStateChanged;
}