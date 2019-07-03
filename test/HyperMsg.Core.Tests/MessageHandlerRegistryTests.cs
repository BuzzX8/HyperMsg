using FakeItEasy;
using System;
using System.Collections.Generic;
using Xunit;

namespace HyperMsg
{
    public class MessageHandlerRegistryTests
    {
        private readonly MessageHandlerRegistry<Guid> publisher = new MessageHandlerRegistry<Guid>();

        [Fact]
        public void Handle_Calls_Registered_Handlers()
        {
            var handlers = A.CollectionOfFake<Action<Guid>>(4);
            AddHandlers(handlers);
            var expected = Guid.NewGuid();

            publisher.Handle(expected);

            foreach (var handler in handlers)
            {
                A.CallTo(() => handler.Invoke(expected)).MustHaveHappened();
            }
        }

        private void AddHandlers(IEnumerable<Action<Guid>> handlers)
        {
            foreach (var handler in handlers)
            {
                publisher.Register(handler);
            }
        }
    }
}