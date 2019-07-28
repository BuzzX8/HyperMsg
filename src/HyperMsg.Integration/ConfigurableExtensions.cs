using System;

namespace HyperMsg.Integration
{
    public static class ConfigurableExtensions
    {
        public static void UseGuidSerializer(this IConfigurable configurable) => configurable.RegisterService(typeof(ISerializer<Guid>), (p, s) => new GuidSerializer());
    }
}
