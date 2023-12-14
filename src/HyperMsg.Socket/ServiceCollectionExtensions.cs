using Microsoft.Extensions.DependencyInjection;

namespace HyperMsg.Socket;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSocketService(this IServiceCollection services) => services.AddSocketService(new MessageBroker());

    public static IServiceCollection AddSocketService(this IServiceCollection services, ITopic transportTopic)
    {
        return services.AddSingleton<SocketHolder>()
            .AddHostedService(provider =>
            {
                var holder = provider.GetRequiredService<SocketHolder>();
                return new ConnectionService(transportTopic, holder);
            })
            .AddHostedService(provider =>
            {
                var holder = provider.GetRequiredService<SocketHolder>();
                return new TransmissionService(transportTopic, holder);
            })
            .AddHostedService(provider =>
            {
                var coderGateway = provider.GetRequiredService<ICoderGateway>();
                return new SocketService(transportTopic, coderGateway);
            });
    }
}
