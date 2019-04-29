using System;
using System.Collections.Generic;

namespace HyperMsg
{
    public delegate IServiceProvider ServiceProviderFactory(IEnumerable<ServiceDescriptor> serviceDescriptors);
}
