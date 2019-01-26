using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg.Transciever
{
    public class MessageTransceiverTests
    {
        private readonly IMessageBuffer<Guid> messageBuffer;
        private readonly SetHandlerAction<Guid> setHandlerAction;
        private MessageTransceiver<Guid> transceiver;
        private List<Func<IDisposable>> runners;

        public MessageTransceiverTests()
        {
            messageBuffer = A.Fake<IMessageBuffer<Guid>>();
            runners = new List<Func<IDisposable>>(A.CollectionOfFake<Func<IDisposable>>(10));
            setHandlerAction = A.Fake<SetHandlerAction<Guid>>();
            transceiver = new MessageTransceiver<Guid>(messageBuffer, setHandlerAction, runners);
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
            transceiver.Run();

            foreach(var runner in runners)
            {
                A.CallTo(() => runner.Invoke()).MustHaveHappened();
            }
        }
    }
}