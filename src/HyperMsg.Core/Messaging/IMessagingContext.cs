namespace HyperMsg.Messaging;

/// <summary>
/// Provides the messaging context, exposing dispatcher and handler registry services
/// for message dispatching and handler management within the messaging infrastructure.
/// </summary>
public interface IMessagingContext
{
    /// <summary>
    /// Gets the dispatcher responsible for sending messages to registered handlers.
    /// </summary>
    IDispatcher Dispatcher { get; }

    /// <summary>
    /// Gets the registry for registering and unregistering message handlers.
    /// </summary>
    IHandlerRegistry HandlerRegistry { get; }
}
