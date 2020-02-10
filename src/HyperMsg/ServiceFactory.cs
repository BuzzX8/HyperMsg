using System;
using System.Collections.Generic;

namespace HyperMsg
{
    /// <summary>
    /// References method which creates certain type of service.
    /// </summary>
    /// <param name="serviceProvider">Provider for dependencies.</param>
    /// <param name="settings">Dictionary which contains named settings.</param>
    /// <returns></returns>
    public delegate object ServiceFactory(IServiceProvider serviceProvider, IConfigurationSettings settings);
}
