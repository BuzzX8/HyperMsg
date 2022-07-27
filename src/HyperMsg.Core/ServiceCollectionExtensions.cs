using Microsoft.Extensions.DependencyInjection;

namespace HyperMsg;

public static class ServiceCollectionExtensions
{
    public const int DefaultBufferSize = 65 * 1024;

    public static IServiceCollection AddSerializationFilter(this IServiceCollection services, int serializationBufferSize = DefaultBufferSize) =>
        services.AddSerializationFilter(BufferFactory.Shared.CreateBuffer(serializationBufferSize));

    public static IServiceCollection AddSerializationFilter(this IServiceCollection services, IBuffer serializationBuffer)
    {
        return services.AddSingleton<SerializationFilter>();
    }

    public static IServiceCollection AddSendBufferFilter(this IServiceCollection services) => services.AddSingleton<SendingPipeline>();
}
