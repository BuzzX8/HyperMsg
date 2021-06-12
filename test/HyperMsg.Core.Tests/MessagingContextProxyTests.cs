using FakeItEasy;
using System;
using System.Threading;
using Xunit;

namespace HyperMsg
{
    public class MessagingContextProxyTests
    {
        private readonly IMessagingContext messagingContext;
        private readonly MessagingContextProxy proxy;

        public MessagingContextProxyTests()
        {
            messagingContext = A.Fake<IMessagingContext>();
            proxy = new MessagingContextProxyMock(messagingContext);
        }

        [Fact]
        public void RegisterHandler_Registers_Handler_With_MessagingContext()
        {
            var handler = A.Fake<Action<string>>();

            proxy.RegisterHandler(handler);

            A.CallTo(() => messagingContext.HandlersRegistry.RegisterHandler(handler)).MustHaveHappened();
        }

        [Fact]
        public void RegisterHandler_Registers_Async_Handler_With_MessagingContext()
        {
            var handler = A.Fake<AsyncAction<string>>();

            proxy.RegisterHandler(handler);

            A.CallTo(() => messagingContext.HandlersRegistry.RegisterHandler(handler)).MustHaveHappened();
        }

        [Fact]
        public void Send_Sends_Message_With_MessagingContext()
        {
            var message = Guid.NewGuid();

            proxy.Send(message);

            A.CallTo(() => messagingContext.Sender.Send(message)).MustHaveHappened();
        }

        [Fact]
        public void SendAsync_Sends_Message_With_MessagingContext()
        {
            var message = Guid.NewGuid();
            var token = new CancellationToken();

            proxy.SendAsync(message, token);

            A.CallTo(() => messagingContext.Sender.SendAsync(message, token)).MustHaveHappened();
        }
    }

    internal class MessagingContextProxyMock : MessagingContextProxy
    {
        public MessagingContextProxyMock(IMessagingContext messagingContext) : base(messagingContext)
        {
        }
    }
}
