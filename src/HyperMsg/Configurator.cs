using System;
using System.Collections.Generic;

namespace HyperMsg
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <param name="settings"></param>
    public delegate void Configurator(IServiceProvider serviceProvider, IReadOnlyDictionary<string, object> settings);
}