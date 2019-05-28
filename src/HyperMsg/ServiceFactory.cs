using System;
using System.Collections.Generic;

namespace HyperMsg
{
    public delegate object ServiceFactory(IServiceProvider serviceProvider, IReadOnlyDictionary<string, object> settings);
}
