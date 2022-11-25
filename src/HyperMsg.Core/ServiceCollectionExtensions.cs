using Microsoft.Extensions.DependencyInjection;

namespace HyperMsg;

public static class ServiceCollectionExtensions
{
    public const int DefaultBufferSize = 65 * 1024;

    public static IServiceCollection AddCodingService(this IServiceCollection services, Decoder deserializer, IEncoder serializer, int bufferSize = DefaultBufferSize) =>
        services.AddCodingService(deserializer, serializer, BufferFactory.Shared.CreateBuffer(bufferSize));

    public static IServiceCollection AddCodingService(this IServiceCollection services, Decoder deserializer, IEncoder serializer, IBuffer encodingBuffer)
    {
        var service = new CodingService(deserializer, serializer, encodingBuffer);

        return services.AddSingleton<CodingService>(service)
            .AddSingleton<ICoderGateway>(service);
    }
}
