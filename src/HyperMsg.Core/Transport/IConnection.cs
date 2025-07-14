namespace HyperMsg.Transport;

/// <summary>
/// Represents a connection that can be opened and closed, and provides state and error notifications.
/// </summary>
public interface IConnection
{
    /// <summary>
    /// Asynchronously opens the connection.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous open operation.</returns>
    ValueTask OpenAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously closes the connection.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous close operation.</returns>
    ValueTask CloseAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Gets the current state of the connection.
    /// </summary>
    ConnectionState State { get; }

    /// <summary>
    /// Occurs when an error is encountered on the connection.
    /// </summary>
    event Action<Exception> OnError;

    /// <summary>
    /// Occurs when the connection state changes.
    /// </summary>
    event Action<ConnectionState> StateChanged;
}

/// <summary>
/// Defines the possible states of a connection.
/// </summary>
public enum ConnectionState
{
    /// <summary>
    /// The connection is not established.
    /// </summary>
    Disconnected,
    /// <summary>
    /// The connection is in the process of being established.
    /// </summary>
    Connecting,
    /// <summary>
    /// The connection is established.
    /// </summary>
    Connected,
    /// <summary>
    /// The connection is in the process of being closed.
    /// </summary>
    Disconnecting
}
