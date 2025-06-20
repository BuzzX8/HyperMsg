using Microsoft.Extensions.DependencyInjection;

namespace HyperMsg.Messaging;

public static class MessagingExtensions
{
    public static IServiceCollection AdMessagingContext(this IServiceCollection services)
    {
        var messageBroker = new MessageBroker();
        services.AddSingleton<IDispatcher>(messageBroker);
        services.AddSingleton<IHandlerRegistry>(messageBroker);
        services.AddSingleton<IMessagingContext>(messageBroker);
        return services;
    }
}
