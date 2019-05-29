using System;
using System.Collections.Generic;

namespace HyperMsg
{
    public delegate void Configurator(IServiceProvider serviceProvider, IReadOnlyDictionary<string, object> settings);
}
