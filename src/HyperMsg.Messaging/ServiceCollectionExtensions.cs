using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HyperMsg.Messaging;

public static class ServiceCollectionExtensions
{
    public static MessagingContextBuilder AddMessagingContext(this IServiceCollection services)
    {
        services.TryAddSingleton(sp =>
        {
            var handlers = sp.GetServices<MessagingContextConfigurator>();
            var broker = new MessageBroker();

            foreach (var configure in handlers)
            {
                configure(broker);
            }

            return broker;
        });
        services.TryAddSingleton<IMessagingContext>(sp => sp.GetRequiredService<MessageBroker>());
        services.TryAddSingleton<IDispatcher>(sp => sp.GetRequiredService<MessageBroker>());
        services.TryAddSingleton<IHandlerRegistry>(sp => sp.GetRequiredService<MessageBroker>());

        return new(services);
    }
}