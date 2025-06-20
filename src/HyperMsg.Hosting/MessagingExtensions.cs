using HyperMsg.Messaging;

namespace HyperMsg.Hosting;

public static class MessagingExtensions
{
    public static IServiceCollection AddMessagingWorker(this IServiceCollection services)
    {
        return services
            .AdMessagingContext()
            .AddHostedService<MessagingWorker>();
    }
}
