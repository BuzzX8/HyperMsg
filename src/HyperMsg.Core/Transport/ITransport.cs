using HyperMsg.Sessions;

namespace HyperMsg.Transport;

/// <summary>
/// Represents a transport abstraction for communication, providing access to connection, streams, session, and reactive events.
/// </summary>
public interface ITransport : IAsyncDisposable
{
    /// <summary>
    /// Gets the underlying connection associated with this transport.
    /// </summary>
    IConnection Connection { get; }

    /// <summary>
    /// Gets the input stream for receiving data.
    /// </summary>
    Stream InputStream { get; }

    /// <summary>
    /// Gets the output stream for sending data.
    /// </summary>
    Stream OutputStream { get; }

    /// <summary>
    /// Gets the session associated with this transport.
    /// </summary>
    ISession Session { get; } // Direct session association

    /// <summary>
    /// Occurs when data is received on the input stream.
    /// The int parameter indicates the number of bytes received.
    /// </summary>
    event Action<int> DataReceived;

    /// <summary>
    /// Occurs when data is sent on the output stream.
    /// The int parameter indicates the number of bytes sent.
    /// </summary>
    event Action<int> DataSent;

    /// <summary>
    /// Occurs when the connection state changes (e.g., connected, disconnected).
    /// </summary>
    event Action<ConnectionState> ConnectionStateChanged;

    /// <summary>
    /// Occurs when a session is created and associated with this transport.
    /// </summary>
    event Action<ISession> SessionCreated;

    /// <summary>
    /// Occurs when a session is closed or expired.
    /// </summary>
    event Action<ISession> SessionClosed;
}