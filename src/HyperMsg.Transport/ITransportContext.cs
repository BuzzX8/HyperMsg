namespace HyperMsg.Transport;

/// <summary>
/// Provides context for a transport operation including the active channel and an optional connection.
/// </summary>
/// <remarks>
/// Implementations expose the <see cref="Channel"/> used for sending and receiving bytes and, when applicable,
/// the higher-level <see cref="IConnection"/> associated with the transport. The <see cref="Connection"/> property
/// may be <c>null</c> for transports that are not connection-oriented or before a connection has been established.
/// </remarks>
public interface ITransportContext
{
    /// <summary>
    /// Gets the connection associated with this transport context, or <c>null</c> if no connection exists.
    /// </summary>
    /// <value>
    /// An <see cref="IConnection"/> instance when the transport is bound to a connection; otherwise <c>null</c>.
    /// </value>
    IConnection? Connection { get; }

    /// <summary>
    /// Gets the channel used for sending and receiving raw bytes.
    /// </summary>
    /// <value>
    /// A non-null <see cref="IChannel"/> instance representing the underlying byte channel.
    /// </value>
    IChannel Channel { get; }
}