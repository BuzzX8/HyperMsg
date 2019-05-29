using System;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg
{
    public class DelegateHandlerTests
    {
        [Fact]
        public void Handle_Calls_HandleAction()
        {
            var expected = Guid.NewGuid();
            var actual = Guid.Empty;
            var handler = new DelegateHandler<Guid>(m => actual = m);

            handler.Handle(expected);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task HandleAsync_Calls_HandleAction_If_HandleAsyncFunction_Not_Provided()
        {
            var expected = Guid.NewGuid();
            var actual = Guid.Empty;
            var handler = new DelegateHandler<Guid>(m => actual = m);

            await handler.HandleAsync(expected);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task HandleAsync_Calls_HandleAsyncFunction_If_Provided()
        {
            var expected = Guid.NewGuid();
            var actual = Guid.Empty;
            var handler = new DelegateHandler<Guid>(_ => { }, (m, t) =>
            {
                actual = m;
                return Task.FromResult(expected);
            });

            await handler.HandleAsync(expected);

            Assert.Equal(expected, actual);
        }
    }
}
