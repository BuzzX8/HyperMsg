using Microsoft.Extensions.DependencyInjection;

namespace HyperMsg.Messaging;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMessagingContext(this IServiceCollection services)
    {
        var messageBroker = new MessageBroker();
        
        return services.AddSingleton<IDispatcher>(messageBroker)
            .AddSingleton<IHandlerRegistry>(messageBroker)
            .AddSingleton<IMessagingContext>(messageBroker);
    }

    public static IServiceCollection AddMessagingComponent<T>(this IServiceCollection services) where T : class, IMessagingComponent 
        => services.AddSingleton<IMessagingComponent, T>();

    public static IServiceCollection AddMessagingComponent<T>(this IServiceCollection services, T component) where T : class, IMessagingComponent 
        => services.AddSingleton<IMessagingComponent>(component);
}
