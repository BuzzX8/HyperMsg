using System.Collections.Generic;

namespace HyperMsg
{
    public class Configuration
    {
        public IList<ServiceDescriptor> Services { get; } = new List<ServiceDescriptor>();

        public IDictionary<string, object> Settings { get; } = new Dictionary<string, object>();
    }
}