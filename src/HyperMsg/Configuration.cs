using System;
using System.Collections.Generic;

namespace HyperMsg
{
    public class Configuration
    {
        public Configuration(ICollection<ServiceDescriptor> services, IReadOnlyDictionary<string, object> settings)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public ICollection<ServiceDescriptor> Services { get; } = new List<ServiceDescriptor>();

        public IReadOnlyDictionary<string, object> Settings { get; } = new Dictionary<string, object>();
    }
}