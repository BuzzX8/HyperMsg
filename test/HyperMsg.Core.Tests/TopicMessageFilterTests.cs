using FakeItEasy;
using System;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg
{
    public class TopicMessageFilterTests : HostFixture
    {
        private readonly StringFilter topicFilter;
        private readonly Func<object, string, bool> filterFunc;

        private readonly Guid topicId = Guid.NewGuid();
        private readonly string message = Guid.NewGuid().ToString();

        public TopicMessageFilterTests()
        {
            filterFunc = A.Fake<Func<object, string, bool>>();
            topicFilter = new StringFilter(MessageSender, filterFunc);
        }

        [Fact]
        public void SendToTopic_Extension_Invokes_Filter_Predicate()
        {
            topicFilter.SendToTopic(topicId, message);

            A.CallTo(() => filterFunc.Invoke(topicId, message)).MustHaveHappened();
        }

        [Fact]
        public async Task SendToTopicAsync_Extension_Invokes_Filter_Predicate()
        {
            await topicFilter.SendToTopicAsync(topicId, message);

            A.CallTo(() => filterFunc.Invoke(topicId, message)).MustHaveHappened();
        }

        [Fact]
        public void SendToTopic_Extension_Does_Not_Sends_Message_If_FilterFunc_Returns_False()
        {
            A.CallTo(() => filterFunc.Invoke(topicId, message)).Returns(false);

            var actualMessage = string.Empty;
            HandlersRegistry.RegisterTopicHandler<string>(topicId, message => actualMessage = message);

            topicFilter.SendToTopic(topicId, message);

            Assert.Empty(actualMessage);
        }

        [Fact]
        public async Task SendToTopicAsync_Extension_Does_Not_Sends_Message_If_FilterFunc_Returns_False()
        {
            A.CallTo(() => filterFunc.Invoke(topicId, message)).Returns(false);

            var actualMessage = string.Empty;
            HandlersRegistry.RegisterTopicHandler<string>(topicId, message => actualMessage = message);

            await topicFilter.SendToTopicAsync(topicId, message);

            Assert.Empty(actualMessage);
        }

        [Fact]
        public void SendToTopic_Extension_Sends_Message_If_FilterFunc_Returns_True()
        {
            A.CallTo(() => filterFunc.Invoke(topicId, message)).Returns(true);

            var actualMessage = string.Empty;
            HandlersRegistry.RegisterTopicHandler<string>(topicId, message => actualMessage = message);

            topicFilter.SendToTopic(topicId, message);

            Assert.Equal(message, actualMessage);
        }

        [Fact]
        public async Task SendToTopicAsync_Extension_Message_If_FilterFunc_Returns_True()
        {
            A.CallTo(() => filterFunc.Invoke(topicId, message)).Returns(true);

            var actualMessage = string.Empty;
            HandlersRegistry.RegisterTopicHandler<string>(topicId, message => actualMessage = message);

            await topicFilter.SendToTopicAsync(topicId, message);

            Assert.Equal(message, actualMessage);
        }
    }

    internal class StringFilter : TopicMessageFilter<string>
    {
        private readonly Func<object, string, bool> filterFunc;

        public StringFilter(ISender messageSender, Func<object, string, bool> filterFunc) : base(messageSender)
            => this.filterFunc = filterFunc;

        protected override bool HandleTopicMessage(object topicId, ref string message) => filterFunc.Invoke(topicId, message);
    }
}
