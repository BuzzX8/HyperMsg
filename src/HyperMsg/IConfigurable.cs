using System;

namespace HyperMsg
{
    /// <summary>
    /// Defines methods for configurable implementation.
    /// </summary>
    public interface IConfigurable
    {
        /// <summary>
        /// Registers initialization method.
        /// </summary>
        /// <param name="initializer">Initializer method which will be invoked during configuration.</param>
        void AddInitializer(Action<IServiceProvider> initializer);

        /// <summary>
        /// Registers factory for specified type of service.
        /// </summary>
        /// <param name="serviceType">Interface implemented by service.</param>
        /// <param name="serviceInstance">Factory method for specified service type.</param>
        void AddService(Type serviceType, object serviceInstance);

        /// <summary>
        /// Registers factory for specified types of service.
        /// </summary>
        /// <param name="serviceType">Interfaces implemented by service.</param>
        /// <param name="serviceFactory">Factory method for specified service type.</param>
        void AddService(Type serviceType, Func<IServiceProvider, object> serviceFactory);
    }
}