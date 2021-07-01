using FakeItEasy;
using System;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg
{
    public class PipeMessageFilterTests : ServiceHostFixture
    {
        private readonly PipeMessageFilter<string> pipeFilter;
        private readonly Func<object, object, string, bool> filterFunc;

        private readonly Guid pipeId = Guid.NewGuid();
        private readonly Guid portId = Guid.NewGuid();
        private readonly string message = Guid.NewGuid().ToString();

        public PipeMessageFilterTests()
        {
            filterFunc = A.Fake<Func<object, object, string, bool>>();
            pipeFilter = new PipeMessageFilter<string>(MessageSender, filterFunc);
        }

        [Fact]
        public void SendToPipe_Extension_Invokes_Filter_Predicate()
        {
            pipeFilter.SendToPipe(pipeId, portId, message);

            A.CallTo(() => filterFunc.Invoke(pipeId, portId, message)).MustHaveHappened();
        }

        [Fact]
        public async Task SendToPipeAsync_Extension_Invokes_Filter_Predicate()
        {
            await pipeFilter.SendToPipeAsync(pipeId, portId, message);

            A.CallTo(() => filterFunc.Invoke(pipeId, portId, message)).MustHaveHappened();
        }

        [Fact]
        public void SendToPipe_Extension_Does_Not_Sends_Message_If_FilterFunc_Returns_False()
        {
            A.CallTo(() => filterFunc.Invoke(pipeId, portId, message)).Returns(false);

            var actualMessage = string.Empty;
            HandlersRegistry.RegisterPipeHandler<string>(pipeId, portId, message => actualMessage = message);

            pipeFilter.SendToPipe(pipeId, portId, message);

            Assert.Empty(actualMessage);
        }

        [Fact]
        public async Task SendToPipeAsync_Extension_Does_Not_Sends_Message_If_FilterFunc_Returns_False()
        {
            A.CallTo(() => filterFunc.Invoke(pipeId, portId, message)).Returns(false);

            var actualMessage = string.Empty;
            HandlersRegistry.RegisterPipeHandler<string>(pipeId, portId, message => actualMessage = message);

            await pipeFilter.SendToPipeAsync(pipeId, portId, message);

            Assert.Empty(actualMessage);
        }

        [Fact]
        public void SendToPipe_Extension_Sends_Message_If_FilterFunc_Returns_True()
        {
            A.CallTo(() => filterFunc.Invoke(pipeId, portId, message)).Returns(true);

            var actualMessage = string.Empty;
            HandlersRegistry.RegisterPipeHandler<string>(pipeId, portId, message => actualMessage = message);

            pipeFilter.SendToPipe(pipeId, portId, message);

            Assert.Equal(message, actualMessage);
        }

        [Fact]
        public async Task SendToPipeAsync_Extension_Message_If_FilterFunc_Returns_True()
        {
            A.CallTo(() => filterFunc.Invoke(pipeId, portId, message)).Returns(true);

            var actualMessage = string.Empty;
            HandlersRegistry.RegisterPipeHandler<string>(pipeId, portId, message => actualMessage = message);

            await pipeFilter.SendToPipeAsync(pipeId, portId, message);

            Assert.Equal(message, actualMessage);
        }
    }
}
