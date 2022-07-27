using Microsoft.Extensions.DependencyInjection;

namespace HyperMsg;

public static class ServiceCollectionExtensions
{
    public const int DefaultBufferSize = 65 * 1024;

    public static IServiceCollection AddCompositeSerializer(this IServiceCollection services)
    {
        var serializer = new CompositeSerializer();
        
        return services.AddSingleton(serializer)
            .AddSingleton<ISerializer>(serializer);
    }

    public static IServiceCollection AddSendingPipeline(this IServiceCollection services, int bufferSize = DefaultBufferSize) =>
        services.AddSendingPipeline(BufferFactory.Shared.CreateBuffer(bufferSize));

    public static IServiceCollection AddSendingPipeline(this IServiceCollection services, IBuffer sendingBuffer)
    {
        return services.AddSingleton(provider =>
        {
            var serializer = provider.GetRequiredService<ISerializer>();
            return new SendingPipeline(serializer, sendingBuffer);
        });
    }
}
