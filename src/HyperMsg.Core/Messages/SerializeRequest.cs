using System;
using System.Collections.Generic;
using System.Text;

namespace HyperMsg.Messages
{
    internal struct SerializeRequest<T>
    {
        public IMessageSender Sender { get; }

        public T Message { get; }
    }
}
