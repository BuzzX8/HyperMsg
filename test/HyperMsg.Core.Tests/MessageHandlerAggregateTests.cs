using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg
{
    public class MessageHandlerAggregateTests
    {
        private readonly MessageHandlerAggregate<Guid> publisher = new MessageHandlerAggregate<Guid>();

        [Fact]
        public async Task HandleAsync_Calls_HandleAsync_For_Each_Registered_Handler()
        {
            var handlers = A.CollectionOfFake<IMessageHandler<Guid>>(4);
            AddHandlers(handlers);
            var expected = Guid.NewGuid();
            var cancellationToken = new CancellationToken();

            await publisher.HandleAsync(expected, cancellationToken);

            foreach (var handler in handlers)
            {
                A.CallTo(() => handler.HandleAsync(expected, cancellationToken)).MustHaveHappened();
            }
        }

        private void AddHandlers(IEnumerable<IMessageHandler<Guid>> handlers)
        {
            foreach (var handler in handlers)
            {
                publisher.Register(handler);
            }
        }
    }
}