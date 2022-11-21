using Microsoft.Extensions.DependencyInjection;

namespace HyperMsg;

public static class ServiceCollectionExtensions
{
    public const int DefaultBufferSize = 65 * 1024;

    public static IServiceCollection AddKernel(this IServiceCollection services, Decoder deserializer, IEncoder serializer, int bufferSize = DefaultBufferSize) =>
        services.AddKernel(deserializer, serializer, BufferFactory.Shared.CreateBuffer(bufferSize));

    public static IServiceCollection AddKernel(this IServiceCollection services, Decoder deserializer, IEncoder serializer, IBuffer sendingBuffer)
    {
        var kernel = new Kernel(deserializer, serializer, sendingBuffer);

        return services.AddSingleton<IDispatcher>(kernel)
            .AddSingleton<IRegistry>(kernel)
            .AddSingleton<ITransportGateway>(kernel);
    }
}
