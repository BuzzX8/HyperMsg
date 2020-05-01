using System;

namespace HyperMsg
{
    /// <summary>
    /// Defines methods for configurable implementation.
    /// </summary>
    public interface IServiceRegistry
    {
        /// <summary>
        /// Adds instance of specified service type.
        /// </summary>
        /// <param name="serviceType">Service type.</param>
        /// <param name="serviceInstance">Instance of service.</param>
        void Add(Type serviceType, object serviceInstance);

        /// <summary>
        /// Adds factory for specified types of service.
        /// </summary>
        /// <param name="serviceType">Service type.</param>
        /// <param name="serviceFactory">Factory method which creates service instance.</param>
        void Add(Type serviceType, Func<IServiceProvider, object> serviceFactory);
    }
}