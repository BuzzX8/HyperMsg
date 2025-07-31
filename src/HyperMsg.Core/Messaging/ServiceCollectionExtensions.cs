using Microsoft.Extensions.DependencyInjection;

namespace HyperMsg.Messaging;

/// <summary>
/// Provides extension methods for registering messaging services and components in the dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers a synchronous message handler for messages of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of message the handler processes.</typeparam>
    /// <param name="services">The service collection to add the handler to.</param>
    /// <param name="messageHandler">The message handler delegate.</param>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddMessageHandler<T>(this IServiceCollection services, MessageHandler<T> messageHandler)
    {
        MessagingContextConfigurator configurator = (context) =>
        {
            context.HandlerRegistry.RegisterHandler(messageHandler);
        };
        return services.AddSingleton(configurator);
    }

    /// <summary>
    /// Registers an asynchronous message handler for messages of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of message the handler processes.</typeparam>
    /// <param name="services">The service collection to add the handler to.</param>        
    /// <param name="messageHandler">The asynchronous message handler delegate.</param>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddAsyncMessageHandler<T>(this IServiceCollection services, AsyncMessageHandler<T> messageHandler)
    {
        MessagingContextConfigurator configurator = (context) =>
        {
            context.HandlerRegistry.RegisterHandler(messageHandler);
        };
        return services.AddSingleton(configurator);
    }

    /// <summary>
    /// Registers the messaging context and its related services (<see cref="IDispatcher"/>, <see cref="IHandlerRegistry"/>, <see cref="IMessagingContext"/>) as singletons in the service collection.
    /// Configurators can be used to register handlers or perform additional setup on the <see cref="IMessagingContext"/>.
    /// </summary>
    /// <param name="services">The service collection to add the messaging context to.</param>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddMessagingContext(this IServiceCollection services)
    {
        return services.AddSingleton<IDispatcher, MessageBroker>(provider =>
        {
            var messageBroker = new MessageBroker();

            var handlers = provider.GetServices<MessagingContextConfigurator>();

            foreach (var handler in handlers)
            {
                handler.Invoke(messageBroker);
            }

            return messageBroker;
        }
        )
            .AddSingleton<IHandlerRegistry, MessageBroker>()
            .AddSingleton<IMessagingContext, MessageBroker>();
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

/// <summary>
/// Delegate for configuring the <see cref="IMessagingContext"/> during service registration.
/// </summary>
/// <param name="messagingContext">The messaging context to configure.</param>
public delegate void MessagingContextConfigurator(IMessagingContext messagingContext);