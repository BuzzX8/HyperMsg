using System;

namespace HyperMsg
{
    /// <summary>
    /// Defines methods for configurable implementation.
    /// </summary>
    public interface IConfigurable
    {
        /// <summary>
        /// Adds initialization method.
        /// </summary>
        /// <param name="initializer">Initializer method which will be invoked during configuration.</param>
        void AddInitializer(Action<IServiceProvider> initializer);

        /// <summary>
        /// Adds instance of specified service type.
        /// </summary>
        /// <param name="serviceType">Service type.</param>
        /// <param name="serviceInstance">Instance of service.</param>
        void AddService(Type serviceType, object serviceInstance);

        /// <summary>
        /// Adds factory for specified types of service.
        /// </summary>
        /// <param name="serviceType">Service type.</param>
        /// <param name="serviceFactory">Factory method which creates service instance.</param>
        void AddService(Type serviceType, Func<IServiceProvider, object> serviceFactory);
    }
}