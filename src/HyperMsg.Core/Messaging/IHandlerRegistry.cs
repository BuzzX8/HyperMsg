namespace HyperMsg.Messaging;

public interface IHandlerRegistry
{
    /// <summary>
    /// Registers a message handler for a specific message type.
    /// </summary>
    /// <typeparam name="T">The type of the message.</typeparam>
    /// <param name="messageHandler">The handler to register.</param>
    void Register<T>(Action<T> messageHandler);

    /// <summary>
    /// Registers a asynchronous message handler for a specific message type.
    /// </summary>
    /// <typeparam name="T">The type of the message.</typeparam>
    /// <param name="asyncMessageHandler">The handler to register.</param>
    void Register<T>(Func<T, CancellationToken, Task> asyncMessageHandler);

    /// <summary>
    /// Unregisters a message handler for a specific message type.
    /// </summary>
    /// <typeparam name="T">The type of the message.</typeparam>
    /// <param name="messageHandler">The handler to unregister.</param>
    void Unregister<T>(Action<T> messageHandler);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">The type of the message.</typeparam>
    /// <param name="asyncMessageHandler">The handler to unregister.</param>
    void Unregister<T>(Func<T, CancellationToken, Task> asyncMessageHandler);
}
