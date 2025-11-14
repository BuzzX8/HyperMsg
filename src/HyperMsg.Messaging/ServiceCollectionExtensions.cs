using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HyperMsg.Messaging;

public static class ServiceCollectionExtensions
{
    public static MessagingContextBuilder AddMessagingContext(this IServiceCollection services)
    {
        services.TryAddSingleton(services =>
        {
            return new MessageBroker();
        });
        services.TryAddSingleton<IMessagingContext>();
        services.TryAddSingleton<IDispatcher, MessageBroker>();
        services.TryAddSingleton<IHandlerRegistry, MessageBroker>();

        return new(services);
    }
}