using Microsoft.Extensions.DependencyInjection;

namespace HyperMsg.Coding;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCodingContext<T>(this IServiceCollection services, Encoder<T> encoder, Decoder<T> decoder)
    {
        return services.AddScoped<ICodingContext<T>>(provider => new CodingContext<T>(encoder, decoder));
    }
}
