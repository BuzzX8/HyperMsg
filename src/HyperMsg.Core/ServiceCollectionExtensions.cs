﻿using Microsoft.Extensions.DependencyInjection;

namespace HyperMsg
{
    public static class ServiceCollectionExtensions
    {
        public const int DefaultBufferSize = 65 * 1024;

        public static IServiceCollection AddContext(this IServiceCollection services) =>
            services.AddSingleton<IContext, Context>();

        public static IServiceCollection AddSerializationFilter(this IServiceCollection services, int serializationBufferSize = DefaultBufferSize) =>
            services.AddSerializationFilter(BufferFactory.Shared.CreateBuffer(serializationBufferSize));

        public static IServiceCollection AddSerializationFilter(this IServiceCollection services, IBuffer serializationBuffer)
        {
            return services.AddSingleton(provider =>
            {
                var context = provider.GetRequiredService<IContext>();
                var sender = context.Sender;
                var filter = new SerializationFilter(context.Sender.Registry, serializationBuffer);
                filter.BufferUpdated += buffer => sender.Dispatch(new BufferUpdatedEvent(buffer));
                return filter;
            });
        }
    }
}