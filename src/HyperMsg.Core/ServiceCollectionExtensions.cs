using Microsoft.Extensions.DependencyInjection;

namespace HyperMsg;

public static class ServiceCollectionExtensions
{
    public const int DefaultBufferSize = 65 * 1024;

    public static IServiceCollection AddPipeline(this IServiceCollection services, int bufferSize = DefaultBufferSize) =>
        services.AddPipeline(BufferFactory.Shared.CreateBuffer(bufferSize));

    public static IServiceCollection AddPipeline(this IServiceCollection services, IBuffer sendingBuffer)
    {
        return services.AddSingleton(provider =>
        {
            var deserializer = provider.GetRequiredService<Deserializer>();
            var serializer = provider.GetRequiredService<ISerializer>();
            return new Pipeline(deserializer, serializer, sendingBuffer);
        });
    }

    public static IServiceCollection AddSerializer(this IServiceCollection services, ISerializer serializer) => services.AddSingleton(serializer);

    public static IServiceCollection AddDeserializer(this IServiceCollection services, Deserializer deserializer) => services.AddSingleton(deserializer);
}
