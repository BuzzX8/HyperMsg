using System;

namespace HyperMsg
{
    public class ServiceDescriptor
    {
        public Func<IServiceProvider, object> ImplementationFactory { get; private set; }

        public object ImplementationInstance { get; private set; }

        public Type ImplementationType { get; private set; }

        public Type ServiceType { get; private set; }

        public static ServiceDescriptor Describe(Type serviceType, Type implementationType)
        {
            return new ServiceDescriptor
            {
                ServiceType = serviceType,
                ImplementationType = implementationType
            };
        }

        public static ServiceDescriptor Describe(Type serviceType, object implementationInstance)
        {
            return new ServiceDescriptor
            {
                ServiceType = serviceType,
                ImplementationInstance = implementationInstance
            };
        }

        public ServiceDescriptor Describe(Type serviceType, Func<IServiceProvider, object> implementationFactory)
        {
            return new ServiceDescriptor
            {
                ServiceType = serviceType,
                ImplementationFactory = implementationFactory
            };                
        }
    }
}
