using System;

namespace HyperMsg
{
    public class ServiceDescriptor
    {
        public Func<IServiceProvider, object> ImplementationFactory { get; }

        public object ImplementationInstance { get; }

        public Type ImplementationType { get; }

        public Type ServiceType { get; }
    }
}
