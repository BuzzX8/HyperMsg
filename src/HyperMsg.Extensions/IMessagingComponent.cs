using System;
using System.Collections.Generic;

namespace HyperMsg
{
    public interface IMessagingComponent
    {
        IEnumerable<IDisposable> Attach(IMessagingContext messagingContext);
    }
}
