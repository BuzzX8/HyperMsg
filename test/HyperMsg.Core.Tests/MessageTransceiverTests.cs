using FakeItEasy;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg.Transciever
{
    public class MessageTransceiverTests
    {
        private readonly IMessageBuffer<Guid> messageBuffer;        
        private readonly IObservable<Guid> observer;
        private MessageTransceiver<Guid> transceiver;

        public MessageTransceiverTests()
        {
            messageBuffer = A.Fake<IMessageBuffer<Guid>>();
            observer = A.Fake<IObservable<Guid>>();
            transceiver = new MessageTransceiver<Guid>(messageBuffer, observer);
        }

        [Fact]
        public void Send_Serializes_Message()
        {
            var message = Guid.NewGuid();

            transceiver.Send(message);

            A.CallTo(() => messageBuffer.Write(message)).MustHaveHappened();
        }

        [Fact]
        public async Task SendAsync_Serializes_Message()
        {
            var message = Guid.NewGuid();

            await transceiver.SendAsync(message);

            A.CallTo(() => messageBuffer.Write(message)).MustHaveHappened();
        }

        [Fact]
        public void Run_Invokes_All_Child_Runners()
        {
            var runners = A.CollectionOfFake<Func<IDisposable>>(10);
            transceiver = new MessageTransceiver<Guid>(messageBuffer, observer, runners.ToArray());

            transceiver.Run();

            foreach(var runner in runners)
            {
                A.CallTo(() => runner.Invoke()).MustHaveHappened();
            }
        }
    }
}