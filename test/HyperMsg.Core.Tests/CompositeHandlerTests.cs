using FakeItEasy;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg
{
    public class CompositeHandlerTests
    {
        private readonly IHandlerRepository handlerRepository;
        private readonly CompositeHandler handler;

        public CompositeHandlerTests()
        {
            handlerRepository = A.Fake<IHandlerRepository>();
            handler = new CompositeHandler(handlerRepository);
        }

        [Fact]
        public void Handle_Calls_Handle_For_Each_Handler_From_Repository()
        {
            var handlers = A.CollectionOfFake<IHandler<string>>(4);
            var expected = Guid.NewGuid().ToString();
            A.CallTo(() => handlerRepository.GetHandlers<string>()).Returns(handlers);

            handler.Handle(expected);

            foreach (var handler in handlers)
            {
                A.CallTo(() => handler.Handle(expected));
            }
        }

        [Fact]
        public void Handle_Throws_Exception_If_No_Handlers_Found()
        {
            A.CallTo(() => handlerRepository.GetHandlers<string>()).Returns(null);

            Assert.Throws<InvalidOperationException>(() => handler.Handle(""));
        }

        [Fact]
        public async Task HandleAsync_Calls_Handle_For_Each_Handler_From_Repository()
        {
            var handlers = A.CollectionOfFake<IHandler<string>>(4);
            var expected = Guid.NewGuid().ToString();
            var cancellationToken = new CancellationToken();
            A.CallTo(() => handlerRepository.GetHandlers<string>()).Returns(handlers);

            await handler.HandleAsync(expected, cancellationToken);

            foreach (var handler in handlers)
            {
                A.CallTo(() => handler.HandleAsync(expected, cancellationToken));
            }
        }

        [Fact]
        public async Task HandleAsync_Throws_Exception_If_No_Handlers_Found()
        {
            A.CallTo(() => handlerRepository.GetHandlers<string>()).Returns(null);

            await Assert.ThrowsAsync<InvalidOperationException>(() => handler.HandleAsync(""));
        }
    }
}
