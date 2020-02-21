using System;

namespace HyperMsg
{
    /// <summary>
    /// Represents method which performs configuration.
    /// </summary>
    /// <param name="serviceProvider">Provider for dependencies.</param>
    /// <param name="settings">Dictionary for settings.</param>
    public delegate void Configurator(IServiceProvider serviceProvider, IConfigurationSettings settings);
}