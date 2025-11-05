namespace HyperMsg.Messaging;

/// <summary>
/// Represents a component that can participate in the messaging system by
/// attaching to an <see cref="IMessagingContext"/> to register handlers and
/// detaching to release resources or unregister handlers.
/// </summary>
public interface IMessagingComponent
{
    /// <summary>
    /// Attach the component to the provided messaging context. Implementations
    /// should register any required message handlers or other callbacks against
    /// <paramref name="messagingContext"/> during attachment.
    /// </summary>
    /// <param name="messagingContext">The messaging context that exposes the dispatcher
    /// and handler registry used by the component.</param>
    void Attach(IMessagingContext messagingContext);

    /// <summary>
    /// Detach the component from the provided messaging context. Implementations
    /// should unregister handlers and perform any necessary cleanup so the component
    /// no longer receives messages from <paramref name="messagingContext"/>.
    /// </summary>
    /// <param name="messagingContext">The messaging context the component is detaching from.</param>
    void Detach(IMessagingContext messagingContext);
}
