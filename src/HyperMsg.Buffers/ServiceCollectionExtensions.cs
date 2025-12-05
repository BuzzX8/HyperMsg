using Microsoft.Extensions.DependencyInjection;

namespace HyperMsg.Buffers;

/// <summary>
/// Provides extension methods for registering buffer-related services in the <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the <see cref="IBufferingContext"/> service with default buffer sizes in the service collection.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance so that additional calls can be chained.</returns>
    public static IServiceCollection AddBufferingContext(this IServiceCollection services) =>        
        services.AddScoped<IBufferingContext, BufferingContext>();
}
