using Microsoft.Extensions.DependencyInjection;
using System.Buffers;

namespace HyperMsg.Buffers;

public static class ServiceCollectionExtensions
{
    // add BufferingContext to the service collection
    public static IServiceCollection AddBufferingContext(this IServiceCollection services)
    {        
        // Register BufferingContext as a singleton service
        services.AddScoped<IBufferingContext, BufferingContext>(services =>
        {
            // Create a new instance of BufferingContext using the service provider
            var memoryOwner = services.GetRequiredService<IMemoryOwner<byte>>();
            return new BufferingContext(memoryOwner.Memory);
        }).AddScoped(services =>
        {
            // Create a new memory owner using the shared memory pool
            return MemoryPool<byte>.Shared.Rent(MemoryPool<byte>.Shared.MaxBufferSize);
        });
        
        // Return the service collection for chaining
        return services;
    }
}
