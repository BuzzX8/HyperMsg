namespace HyperMsg.Transport;

public interface IConnection
{
    Task OpenAsync(CancellationToken cancellationToken);

    Task CloseAsync(CancellationToken cancellationToken);

    ConnectionState State { get; }

    event Action<Exception> OnError;

    event Action<ConnectionState> StateChanged;
}

public enum ConnectionState
{
    Disconnected,
    Connecting,
    Connected,
    Disconnecting
}
