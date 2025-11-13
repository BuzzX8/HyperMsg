using HyperMsg.Messaging;

namespace HyperMsg.Hosting;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMessagingWorker(this IServiceCollection services)
    {
        return services;
            //.AddHostedService<MessagingWorker>()
            //.AddMessagingContext();
    }
}
