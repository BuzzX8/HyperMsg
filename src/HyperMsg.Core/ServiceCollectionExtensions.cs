using Microsoft.Extensions.DependencyInjection;

namespace HyperMsg
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddContext(this IServiceCollection services) =>
            services.AddSingleton(new Context());

        public static IServiceCollection AddSerializationFilter(this IServiceCollection services, int serializationBufferSize) =>
            services.AddSerializationFilter(BufferFactory.Shared.CreateBuffer(serializationBufferSize));

        public static IServiceCollection AddSerializationFilter(this IServiceCollection services, IBuffer serializationBuffer)
        {
            return services.AddSingleton(provider =>
            {
                var context = provider.GetRequiredService<IContext>();
                return new SerializationFilter(context.Sender.Registry, serializationBuffer);
            });
        }
    }
}
