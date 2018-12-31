using System;
using System.Collections.Generic;

namespace HyperMsg
{
    public class BuilderContext
    {
        public IServiceProvider ServiceProvider { get; }

        public IList<ServiceDescriptor> Services { get; }

        public IDictionary<string, object> Settings { get; }
    }
}