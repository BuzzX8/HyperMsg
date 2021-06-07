using System;

namespace HyperMsg.Messages
{
    internal class WaitMessageRequest
    {
        public WaitMessageRequest(Func<object, bool> messagePredicate) => MessagePredicate = messagePredicate;

        public Func<object, bool> MessagePredicate { get; }

        public object Message { get; set; }
    }
}
