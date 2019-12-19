using System;
using System.Collections.Generic;

namespace HyperMsg
{
    /// <summary>
    /// Defines methods for configurable implementation.
    /// </summary>
    public interface IConfigurable
    {
        /// <summary>
        /// Adds named setting.
        /// </summary>
        /// <param name="settingName">Name of setting.</param>
        /// <param name="setting">Setting value.</param>
        void AddSetting(string settingName, object setting);

        /// <summary>
        /// Registers configuration method.
        /// </summary>
        /// <param name="configurator">Configurator method which will be invoked during configuration.</param>
        void RegisterConfigurator(Configurator configurator);

        /// <summary>
        /// Registers factory for specified type of service.
        /// </summary>
        /// <param name="serviceType">Interface implemented by service.</param>
        /// <param name="serviceFactory">Factory method for specified service type.</param>
        void RegisterService(Type serviceType, ServiceFactory serviceFactory);

        /// <summary>
        /// Registers factory for specified types of service.
        /// </summary>
        /// <param name="serviceTypes">Interfaces implemented by service.</param>
        /// <param name="serviceFactory">Factory method for specified service type.</param>
        void RegisterService(IEnumerable<Type> serviceTypes, ServiceFactory serviceFactory);
    }
}