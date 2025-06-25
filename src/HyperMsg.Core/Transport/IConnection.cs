namespace HyperMsg.Transport;

public interface IConnection
{
    Task OpenAsync(CancellationToken cancellationToken);

    Task CloseAsync(CancellationToken cancellationToken);

    ConnectionState State { get; }

    event Action<Exception> OnError;

    event Action<ConnectionState> ConnectionStateChanged;
}

public enum ConnectionState
{
    Disconnected,
    Connecting,
    Connected,
    Disconnecting
}
