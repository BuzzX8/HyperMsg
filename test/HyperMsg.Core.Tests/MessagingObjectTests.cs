using FakeItEasy;
using System;
using System.Threading;
using Xunit;

namespace HyperMsg
{
    public class MessagingObjectTests
    {
        private readonly IMessagingContext messagingContext;
        private readonly MessagingObjectMock messagingObject;

        public MessagingObjectTests()
        {
            messagingContext = A.Fake<IMessagingContext>();
            messagingObject = new MessagingObjectMock(messagingContext);
        }

        [Fact]
        public void RegisterHandler_Registers_Handler_With_MessagingContext()
        {
            var handler = A.Fake<Action<string>>();

            messagingObject.RegisterHandler(handler);

            A.CallTo(() => messagingContext.HandlersRegistry.RegisterHandler(handler)).MustHaveHappened();
        }

        [Fact]
        public void RegisterHandler_Registers_Async_Handler_With_MessagingContext()
        {
            var handler = A.Fake<AsyncAction<string>>();

            messagingObject.RegisterHandler(handler);

            A.CallTo(() => messagingContext.HandlersRegistry.RegisterHandler(handler)).MustHaveHappened();
        }

        [Fact]
        public void Send_Sends_Message_With_MessagingContext()
        {
            var message = Guid.NewGuid();

            messagingObject.Send(message);

            A.CallTo(() => messagingContext.Sender.Send(message)).MustHaveHappened();
        }

        [Fact]
        public void SendAsync_Sends_Message_With_MessagingContext()
        {
            var message = Guid.NewGuid();
            var token = new CancellationToken();

            messagingObject.SendAsync(message, token);

            A.CallTo(() => messagingContext.Sender.SendAsync(message, token)).MustHaveHappened();
        }
    }

    public class MessagingObjectMock : MessagingObject
    {
        public MessagingObjectMock(IMessagingContext messagingContext) : base(messagingContext)
        {
        }
    }
}
