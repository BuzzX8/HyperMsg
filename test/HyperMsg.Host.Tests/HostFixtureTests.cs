using Xunit;

namespace HyperMsg
{
    public class HostFixtureTests : HostFixture
    {
        [Fact]
        public void Sender_Context_Transfers_Message()
        {
            var message = Guid.NewGuid();
            var actual = Guid.Empty;
            SenderRegistry.Register<Guid>(m => actual = m);

            Sender.Dispatch(message);

            Assert.Equal(message, actual);
        }

        [Fact]
        public void Receiver_Context_Transfers_Message()
        {
            var message = Guid.NewGuid();
            var actual = Guid.Empty;
            ReceiverRegistry.Register<Guid>(m => actual = m);
            
            Receiver.Dispatch(message);

            Assert.Equal(message, actual);
        }
    }
}
