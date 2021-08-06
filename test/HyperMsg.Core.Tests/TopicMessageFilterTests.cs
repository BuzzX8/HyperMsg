using FakeItEasy;
using System;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg
{
    public class TopicMessageFilterTests : HostFixture
    {
        private readonly TopicMessageFilter<string> TopicFilter;
        private readonly Func<object, string, bool> filterFunc;

        private readonly Guid TopicId = Guid.NewGuid();
        private readonly string message = Guid.NewGuid().ToString();

        public TopicMessageFilterTests()
        {
            filterFunc = A.Fake<Func<object, string, bool>>();
            TopicFilter = new TopicMessageFilter<string>(MessageSender, filterFunc);
        }

        [Fact]
        public void SendToTopic_Extension_Invokes_Filter_Predicate()
        {
            TopicFilter.SendToTopic(TopicId, message);

            A.CallTo(() => filterFunc.Invoke(TopicId, message)).MustHaveHappened();
        }

        [Fact]
        public async Task SendToTopicAsync_Extension_Invokes_Filter_Predicate()
        {
            await TopicFilter.SendToTopicAsync(TopicId, message);

            A.CallTo(() => filterFunc.Invoke(TopicId, message)).MustHaveHappened();
        }

        [Fact]
        public void SendToTopic_Extension_Does_Not_Sends_Message_If_FilterFunc_Returns_False()
        {
            A.CallTo(() => filterFunc.Invoke(TopicId, message)).Returns(false);

            var actualMessage = string.Empty;
            HandlersRegistry.RegisterTopicHandler<string>(TopicId, message => actualMessage = message);

            TopicFilter.SendToTopic(TopicId, message);

            Assert.Empty(actualMessage);
        }

        [Fact]
        public async Task SendToTopicAsync_Extension_Does_Not_Sends_Message_If_FilterFunc_Returns_False()
        {
            A.CallTo(() => filterFunc.Invoke(TopicId, message)).Returns(false);

            var actualMessage = string.Empty;
            HandlersRegistry.RegisterTopicHandler<string>(TopicId, message => actualMessage = message);

            await TopicFilter.SendToTopicAsync(TopicId, message);

            Assert.Empty(actualMessage);
        }

        [Fact]
        public void SendToTopic_Extension_Sends_Message_If_FilterFunc_Returns_True()
        {
            A.CallTo(() => filterFunc.Invoke(TopicId, message)).Returns(true);

            var actualMessage = string.Empty;
            HandlersRegistry.RegisterTopicHandler<string>(TopicId, message => actualMessage = message);

            TopicFilter.SendToTopic(TopicId, message);

            Assert.Equal(message, actualMessage);
        }

        [Fact]
        public async Task SendToTopicAsync_Extension_Message_If_FilterFunc_Returns_True()
        {
            A.CallTo(() => filterFunc.Invoke(TopicId, message)).Returns(true);

            var actualMessage = string.Empty;
            HandlersRegistry.RegisterTopicHandler<string>(TopicId, message => actualMessage = message);

            await TopicFilter.SendToTopicAsync(TopicId, message);

            Assert.Equal(message, actualMessage);
        }
    }
}
