namespace HyperMsg.Messaging;

/// <summary>
/// Base implementation of <see cref="IMessagingComponent"/> that manages
/// registration of message handlers and disposal of the resulting registrations.
/// Derived types should implement <see cref="RegisterHandlers(IMessagingContext)"/>
/// to register handlers against the provided <see cref="IMessagingContext"/> and
/// return <see cref="IDisposable"/> tokens that will be disposed when the
/// component is detached or disposed.
/// </summary>
public abstract class MessagingComponentBase : IMessagingComponent, IDisposable
{
    /// <summary>
    /// Disposables returned from <see cref="RegisterHandlers(IMessagingContext)"/>.
    /// Disposing these should unregister any handlers or free resources allocated
    /// during registration.
    /// </summary>
    private readonly List<IDisposable> disposables = [];

    /// <summary>
    /// Attaches the component to the specified <paramref name="messagingContext"/>.
    /// Calls <see cref="RegisterHandlers(IMessagingContext)"/> and stores the returned
    /// disposables so they can be disposed when the component is detached.
    /// </summary>
    /// <param name="messagingContext">The messaging context used to register handlers.</param>
    public void Attach(IMessagingContext messagingContext)
    {
        disposables.AddRange(RegisterHandlers(messagingContext));
    }

    /// <summary>
    /// Detaches the component from the messaging context. The default behavior is
    /// to dispose the stored registration disposables, which should unregister handlers.
    /// </summary>
    /// <param name="_">The messaging context this component is detaching from.</param>
    public void Detach(IMessagingContext _) => Dispose();

    /// <summary>
    /// Implementations must register required handlers against the provided
    /// <paramref name="messagingContext"/> and return <see cref="IDisposable"/>
    /// instances that will unregister those handlers when disposed.
    /// </summary>
    /// <param name="messagingContext">The messaging context the component should use to register handlers.</param>
    /// <returns>A sequence of <see cref="IDisposable"/> tokens representing the registrations.</returns>
    protected abstract IEnumerable<IDisposable> RegisterHandlers(IMessagingContext messagingContext);

    /// <summary>
    /// Disposes all registration disposables collected during <see cref="Attach(IMessagingContext)"/>.
    /// Calling this will typically unregister any handlers previously registered.
    /// </summary>
    public void Dispose()
    {
        foreach (var disposable in disposables)
        {
            disposable.Dispose();
        }
    }
}
