namespace HyperMsg.Messaging;

/// <summary>
/// Defines a registry for registering and unregistering message handlers for specific message types.
/// </summary>
public interface IHandlerRegistry
{
    /// <summary>
    /// Registers a message handler for a specific message type.
    /// </summary>
    /// <typeparam name="T">The type of the message.</typeparam>
    /// <param name="messageHandler">The handler to register.</param>
    void Register<T>(MessageHandler<T> messageHandler);

    /// <summary>
    /// Registers an asynchronous message handler for a specific message type.
    /// </summary>
    /// <typeparam name="T">The type of the message.</typeparam>
    /// <param name="asyncMessageHandler">The handler to register.</param>
    void Register<T>(AsyncMessageHandler<T> asyncMessageHandler);

    /// <summary>
    /// Unregisters a message handler for a specific message type.
    /// </summary>
    /// <typeparam name="T">The type of the message.</typeparam>
    /// <param name="messageHandler">The handler to unregister.</param>
    void Unregister<T>(MessageHandler<T> messageHandler);

    /// <summary>
    /// Unregisters an asynchronous message handler for a specific message type.
    /// </summary>
    /// <typeparam name="T">The type of the message.</typeparam>
    /// <param name="asyncMessageHandler">The handler to unregister.</param>
    void Unregister<T>(AsyncMessageHandler<T> asyncMessageHandler);
}

/// <summary>
/// Represents a synchronous message handler delegate for messages of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of the message.</typeparam>
/// <param name="message">The message instance.</param>
public delegate void MessageHandler<T>(T message);

/// <summary>
/// Represents an asynchronous message handler delegate for messages of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of the message.</typeparam>
/// <param name="message">The message instance.</param>
/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
/// <returns>A task that represents the asynchronous operation.</returns>
public delegate Task AsyncMessageHandler<T>(T message, CancellationToken cancellationToken);