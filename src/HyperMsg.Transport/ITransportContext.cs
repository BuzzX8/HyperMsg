namespace HyperMsg.Transport;

/// <summary>
/// Represents a transport context that manages the connection and data transmission for a transport layer.
/// </summary>
/// <remarks>
/// Implementations of this interface are responsible for handling the underlying connection and providing asynchronous data sending capabilities.
/// </remarks>
public interface ITransportContext
{
    /// <summary>
    /// Gets the connection associated with the transport context.
    /// </summary>
    IConnection? Connection { get; }

    IChannel Channel { get; }
}