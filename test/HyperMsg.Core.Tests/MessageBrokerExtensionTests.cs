using FakeItEasy;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg
{
    public class MessageBrokerExtensionTests
    {
        private readonly MessageBroker broker = new();

        [Fact]
        public void RegisterHandler_Invokes_Handler_If_Predicate_Returns_True()
        {            
            var handler = A.Fake<Action<Guid>>();
            broker.RegisterHandler(_ => true, handler);

            broker.Send(Guid.NewGuid());

            A.CallTo(() => handler.Invoke(A<Guid>._)).MustHaveHappened();
        }

        [Fact]
        public void RegisterHandler_Does_Not_Invokes_Handler_If_Predicate_Returns_False()
        {
            var handler = A.Fake<Action<Guid>>();
            broker.RegisterHandler(_ => false, handler);

            broker.Send(Guid.NewGuid());

            A.CallTo(() => handler.Invoke(A<Guid>._)).MustNotHaveHappened();
        }

        private readonly Guid topicId = Guid.NewGuid();
        private readonly Guid message = Guid.NewGuid();

        [Fact]
        public void SendToTopic_Invokes_Handler_Registered_With_RegisterTopicHandler()
        {
            var filter = A.Fake<Action<Guid>>();
            broker.RegisterTopicHandler(topicId, filter);

            broker.SendToTopic(topicId, message);

            A.CallTo(() => filter.Invoke(message)).MustHaveHappened();
        }

        [Fact]
        public async Task SendToTopicAsync_Invokes_Handler_Registered_With_RegisterTopicHandler()
        {
            var filter = A.Fake<AsyncAction<Guid>>();
            broker.RegisterTopicHandler(topicId, filter);

            await broker.SendToTopicAsync(topicId, message);

            A.CallTo(() => filter.Invoke(message, A<CancellationToken>._)).MustHaveHappened();
        }

        [Fact]
        public void SendToTopic_Does_Not_Invokes_Topic_Handler_With_Different_TopicId()
        {
            var handler = A.Fake<Action<Guid>>();
            broker.RegisterTopicHandler(topicId, handler);

            broker.SendToTopic(Guid.NewGuid(), message);

            A.CallTo(() => handler.Invoke(message)).MustNotHaveHappened();
        }

        [Fact]
        public async Task SendToTopicAsync_Does_Not_Invokes_Topic_Handler_With_Different_TopicId()
        {
            var handler = A.Fake<AsyncAction<Guid>>();
            broker.RegisterTopicHandler(topicId, handler);

            await broker.SendToTopicAsync(Guid.NewGuid(), message);

            A.CallTo(() => handler.Invoke(message, A<CancellationToken>._)).MustNotHaveHappened();
        }

        [Fact]
        public void SendToTransportTopic_Invokes_Handler_Registered_By_RegisterTransportTopicHandler()
        {
            var handler = A.Fake<Action<Guid>>();
            broker.RegisterTransportTopicHandler(handler);

            broker.SendToTransportTopic(message);

            A.CallTo(() => handler.Invoke(message)).MustHaveHappened();
        }

        [Fact]
        public async Task SendToTransportTopicAsync_Invokes_Handler_Registered_By_RegisterTransportTopicHandler()
        {
            var handler = A.Fake<AsyncAction<Guid>>();
            broker.RegisterTransportTopicHandler(handler);

            await broker.SendToTransportTopicAsync(message);

            A.CallTo(() => handler.Invoke(message, A<CancellationToken>._)).MustHaveHappened();
        }

        [Fact]
        public void SendToTransmitTopic_Invokes_Handler_Registered_By_RegisterTransportTopicHandler()
        {
            var handler = A.Fake<Action<Guid>>();
            broker.RegisterTransmitTopicHandler(handler);

            broker.SendToTransmitTopic(message);

            A.CallTo(() => handler.Invoke(message)).MustHaveHappened();
        }

        [Fact]
        public async Task SendToTransmitTopicAsync_Invokes_Handler_Registered_By_RegisterTransportTopicHandler()
        {
            var handler = A.Fake<AsyncAction<Guid>>();
            broker.RegisterTransmitTopicHandler(handler);

            await broker.SendToTransmitTopicAsync(message);

            A.CallTo(() => handler.Invoke(message, A<CancellationToken>._)).MustHaveHappened();
        }

        [Fact]
        public void SendToReceiveTopic_Invokes_Handler_Registered_By_RegisterTransportTopicHandler()
        {
            var handler = A.Fake<Action<Guid>>();
            broker.RegisterReceiveTopicHandler(handler);

            broker.SendToReceiveTopic(message);

            A.CallTo(() => handler.Invoke(message)).MustHaveHappened();
        }

        [Fact]
        public async Task SendToReceiveTopicAsync_Invokes_Handler_Registered_By_RegisterTransportTopicHandler()
        {
            var handler = A.Fake<AsyncAction<Guid>>();
            broker.RegisterReceiveTopicHandler(handler);

            await broker.SendToReceiveTopicAsync(message);

            A.CallTo(() => handler.Invoke(message, A<CancellationToken>._)).MustHaveHappened();
        }
    }
}
