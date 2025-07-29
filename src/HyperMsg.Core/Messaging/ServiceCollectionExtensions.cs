using Microsoft.Extensions.DependencyInjection;

namespace HyperMsg.Messaging;

/// <summary>
/// Provides extension methods for registering messaging services and components in the dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the messaging context and its related services (<see cref="IDispatcher"/>, <see cref="IHandlerRegistry"/>, <see cref="IMessagingContext"/>) as singletons in the service collection.
    /// </summary>
    /// <param name="services">The service collection to add the messaging context to.</param>
    /// <param name="configurator">An optional delegate to configure the <see cref="IMessagingContext"/> instance.</param>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddMessagingContext(this IServiceCollection services, Action<IMessagingContext> configurator = null)
    {        
        var messageBroker = new MessageBroker();

        configurator?.Invoke(messageBroker);

        return services.AddSingleton<IDispatcher>(messageBroker)
            .AddSingleton<IHandlerRegistry>(messageBroker)
            .AddSingleton<IMessagingContext>(messageBroker);
    }

    /// <summary>
    /// Registers a messaging component of type <typeparamref name="T"/> as a singleton implementation of <see cref="IMessagingComponent"/>.
    /// </summary>
    /// <typeparam name="T">The type of the messaging component to register.</typeparam>
    /// <param name="services">The service collection to add the messaging component to.</param>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddMessagingComponent<T>(this IServiceCollection services) where T : class, IMessagingComponent 
        => services.AddSingleton<IMessagingComponent, T>();

    /// <summary>
    /// Registers the specified messaging component instance as a singleton implementation of <see cref="IMessagingComponent"/>.
    /// </summary>
    /// <typeparam name="T">The type of the messaging component.</typeparam>
    /// <param name="services">The service collection to add the messaging component to.</param>
    /// <param name="component">The messaging component instance to register.</param>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddMessagingComponent<T>(this IServiceCollection services, T component) where T : class, IMessagingComponent 
        => services.AddSingleton<IMessagingComponent>(component);
}
