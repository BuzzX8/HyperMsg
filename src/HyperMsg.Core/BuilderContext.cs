using System;
using System.Collections.Generic;

namespace HyperMsg
{
    public class BuilderContext
    {
        public ICollection<Func<IDisposable>> Runners { get; }

        public IServiceProvider ServiceProvider { get; }

        public IList<ServiceDescriptor> Services { get; } = new List<ServiceDescriptor>();

        public IDictionary<string, object> Settings { get; } = new Dictionary<string, object>();
    }
}