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
        private readonly Guid topicId = Guid.NewGuid();
        private readonly Guid message = Guid.NewGuid();

        [Fact]
        public void SendMessage_Invokes_Handler_Registered_With_RegisterMessageHandler()
        {
            var header = Guid.NewGuid().ToString();
            var messageHandler = A.Fake<Action<string, Guid>>();
            broker.RegisterMessageHandler(messageHandler);

            broker.SendMessage(header, message);

            A.CallTo(() => messageHandler.Invoke(header, message)).MustHaveHappened();
        }

        [Fact]
        public void SendMessage_Invokes_Async_Handler_Registered_With_RegisterMessageHandler()
        {
            var header = Guid.NewGuid().ToString();
            var messageHandler = A.Fake<AsyncAction<string, Guid>>();
            broker.RegisterMessageHandler(messageHandler);

            broker.SendMessage(header, message);

            A.CallTo(() => messageHandler.Invoke(header, message, A<CancellationToken>._)).MustHaveHappened();
        }

        [Fact]
        public void SendCommand_Invokes_Handler_Registered_With_RegisterCommandHandler()
        {
            var commandHandler = A.Fake<Action<Guid>>();
            broker.RegisterCommandHandler(commandHandler);

            broker.SendCommand(message);

            A.CallTo(() => commandHandler.Invoke(message)).MustHaveHappened();
        }

        [Fact]
        public void SendCommandAsync_Invokes_Handler_Registered_With_RegisterCommandHandler()
        {
            var commandHandler = A.Fake<AsyncAction<Guid>>();
            broker.RegisterCommandHandler(commandHandler);

            broker.SendCommand(message);

            A.CallTo(() => commandHandler.Invoke(message, A<CancellationToken>._)).MustHaveHappened();
        }

        [Fact]
        public void SendEvent_Invokes_Handler_Registered_With_RegisterEventHandler()
        {
            var eventHandler = A.Fake<Action<Guid>>();
            broker.RegisterEventHandler(eventHandler);

            broker.SendEvent(message);

            A.CallTo(() => eventHandler.Invoke(message)).MustHaveHappened();
        }

        [Fact]
        public async Task SendEventAsync_Invokes_Handler_Registered_With_RegisterEventHandler()
        {
            var eventHandler = A.Fake<AsyncAction<Guid>>();
            broker.RegisterEventHandler(eventHandler);

            await broker.SendEventAsync(message);

            A.CallTo(() => eventHandler.Invoke(message, A<CancellationToken>._)).MustHaveHappened();
        }

        [Fact]
        public void SendTransmitCommand_Invokes_Handler_Registered_With_RegisterTransmitCommandHandler()
        {
            var commandHandler = A.Fake<Action<Guid>>();
            broker.RegisterTransmitCommandHandler(commandHandler);

            broker.SendTransmitCommand(message);

            A.CallTo(() => commandHandler.Invoke(message)).MustHaveHappened();
        }

        [Fact]
        public async Task SendTransmitCommandAsync_Invokes_Handler_Registered_With_RegisterTransmitCommandHandler()
        {
            var commandHandler = A.Fake<AsyncAction<Guid>>();
            broker.RegisterTransmitCommandHandler(commandHandler);

            await broker.SendTransmitCommandAsync(message);

            A.CallTo(() => commandHandler.Invoke(message, A<CancellationToken>._)).MustHaveHappened();
        }

        [Fact]
        public void SendReceiveEvent_Invokes_Handler_Registered_With_RegisterReceiveEventHandler()
        {
            var commandHandler = A.Fake<Action<Guid>>();
            broker.RegisterReceiveEventHandler(commandHandler);

            broker.SendReceiveEvent(message);

            A.CallTo(() => commandHandler.Invoke(message)).MustHaveHappened();
        }

        [Fact]
        public async Task SendReceiveEventAsync_Invokes_Handler_Registered_With_RegisterReceiveEventHandler()
        {
            var commandHandler = A.Fake<AsyncAction<Guid>>();
            broker.RegisterReceiveEventHandler(commandHandler);

            await broker.SendReceiveEventAsync(message);

            A.CallTo(() => commandHandler.Invoke(message, A<CancellationToken>._)).MustHaveHappened();
        }
    }
}