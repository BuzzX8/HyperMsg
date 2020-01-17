using FakeItEasy;
using System;
using Xunit;

namespace HyperMsg
{
    public class MessageSenderExtensionsTests
    {
        private readonly IMessageSender messageSender = A.Fake<IMessageSender>();
        private readonly string message = Guid.NewGuid().ToString();

        [Fact]
        public void Received_Sends_Message_Decorated_With_Received()
        {
            messageSender.Received(message);

            A.CallTo(() => messageSender.Send<string>(new Received<string>(message))).MustHaveHappened();
        }
    }
}
