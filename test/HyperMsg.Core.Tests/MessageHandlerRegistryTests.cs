using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg
{
    public class MessageHandlerRegistryTests
    {
        private readonly MessageHandlerRegistry<Guid> publisher = new MessageHandlerRegistry<Guid>();

        [Fact]
        public async Task HandleAsync_Calls_Registered_Handlers()
        {
            var handlers = A.CollectionOfFake<Action<Guid>>(4);
            AddHandlers(handlers);
            var expected = Guid.NewGuid();

            await publisher.HandleAsync(expected, default);

            foreach (var handler in handlers)
            {
                A.CallTo(() => handler.Invoke(expected)).MustHaveHappened();
            }
        }

        [Fact]
        public async Task HandleAsync_Calls_Registered_Async_Handlers()
        {
            var handlers = A.CollectionOfFake<AsyncAction<Guid>>(4);
            var expected = Guid.NewGuid();

            AddHandlers(handlers);

            await publisher.HandleAsync(expected, default);

            foreach (var handler in handlers)
            {
                A.CallTo(() => handler.Invoke(expected, A<CancellationToken>._)).MustHaveHappened();
            }
        }

        private void AddHandlers(IList<AsyncAction<Guid>> handlers)
        {
            foreach (var handler in handlers)
            {
                publisher.Register(handler);
            }
        }

        private void AddHandlers(IList<Action<Guid>> handlers)
        {
            foreach (var handler in handlers)
            {
                publisher.Register(handler);
            }
        }
    }
}