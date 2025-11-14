using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HyperMsg.Messaging;

public static class ServiceCollectionExtensions
{
    public static MessagingContextBuilder AddMessagingContext(this IServiceCollection services)
    {
        var messageBroker = new MessageBroker();
        
        services.TryAddSingleton<IMessagingContext>(messageBroker);
        services.TryAddSingleton<IDispatcher>(messageBroker);
        services.TryAddSingleton<IHandlerRegistry>(messageBroker);

        return new(services);
    }
}