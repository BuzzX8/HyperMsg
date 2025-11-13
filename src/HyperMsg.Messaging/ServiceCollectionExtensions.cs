using Microsoft.Extensions.DependencyInjection;

namespace HyperMsg.Messaging;

public static class ServiceCollectionExtensions
{
    public static MessagingContextBuilder AddMessagingContext(this IServiceCollection services)
    {
        var messagingContextBuilder = new MessagingContextBuilder();        
               
        services.AddSingleton(sp =>
        {
            return messagingContextBuilder.Build();
        });
        services.AddSingleton(provider => provider.GetRequiredService<IMessagingContext>().Dispatcher);
        services.AddSingleton(provider => provider.GetRequiredService<IMessagingContext>().HandlerRegistry);

        return messagingContextBuilder;
    }
}