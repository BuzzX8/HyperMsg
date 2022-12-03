using Microsoft.Extensions.DependencyInjection;

namespace HyperMsg;

public static class ServiceCollectionExtensions
{
    public const int DefaultBufferSize = 65 * 1024;

    public static IServiceCollection AddCodingService(this IServiceCollection services, Decoder decoder, IEncoder encoder, int decodingBufferSize = DefaultBufferSize, int encodingBufferSize = DefaultBufferSize)
    {
        var service = new CodingService(decoder, encoder, decodingBufferSize, encodingBufferSize);

        return services.AddSingleton(service)
            .AddSingleton<ICoderGateway>(service);
    }
}
