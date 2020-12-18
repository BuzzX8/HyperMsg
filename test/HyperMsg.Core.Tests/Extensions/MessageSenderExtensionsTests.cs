using FakeItEasy;
using System;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg.Extensions
{
    public class MessageSenderExtensionsTests
    {
        private readonly IMessageSender messageSender = A.Fake<IMessageSender>();
        private readonly string message = Guid.NewGuid().ToString();

        [Fact]
        public void Received_Sends_Message_Decorated_With_Received()
        {
            messageSender.Received(message);

            A.CallTo(() => messageSender.Send(new Received<string>(message))).MustHaveHappened();
        }

        [Fact]
        public async Task ReceivedAsync_Sends_Message_Decorated_With_Received()
        {
            await messageSender.ReceivedAsync(message, default);

            A.CallTo(() => messageSender.SendAsync(new Received<string>(message), default)).MustHaveHappened();
        }

        [Fact]
        public void Transmit_Sends_Message_Decorated_With_Transmit()
        {
            messageSender.Transmit(message);

            A.CallTo(() => messageSender.Send(new Transmit<string>(message))).MustHaveHappened();
        }

        [Fact]
        public async Task TransmitAsync_Sends_Message_Decorated_With_Transmit()
        {
            await messageSender.TransmitAsync(message, default);

            A.CallTo(() => messageSender.SendAsync(new Transmit<string>(message), default));
        }
    }
}
