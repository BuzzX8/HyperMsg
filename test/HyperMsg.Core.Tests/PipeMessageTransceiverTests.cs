using FakeItEasy;
using System;
using Xunit;

namespace HyperMsg
{
    public class PipeMessageTransceiverTests
    {
        private readonly IMessageBuffer<Guid> messageBuffer;
        private readonly PipeMessageTransceiver<Guid> transceiver;
        private readonly RunAction run1;
        private readonly RunAction run2;
        
        public PipeMessageTransceiverTests()
        {
            messageBuffer = A.Fake<IMessageBuffer<Guid>>();
            run1 = A.Fake<RunAction>();
            run2 = A.Fake<RunAction>();
            transceiver = new PipeMessageTransceiver<Guid>(messageBuffer, run1, run2);
        }

        [Fact]
        public void Run_Invokes_Both_Runners()
        {
            transceiver.Run();

            A.CallTo(() => run1.Invoke()).MustHaveHappened();
            A.CallTo(() => run2.Invoke()).MustHaveHappened();
        }

        [Fact]
        public void Disposes_Both_Runners()
        {
            var runHandle1 = A.Fake<IDisposable>();
            A.CallTo(() => run1.Invoke()).Returns(runHandle1);
            var runHandle2 = A.Fake<IDisposable>();
            A.CallTo(() => run2.Invoke()).Returns(runHandle2);

            var disp = transceiver.Run();

            disp.Dispose();

            A.CallTo(() => runHandle1.Dispose()).MustHaveHappened();
            A.CallTo(() => runHandle2.Dispose()).MustHaveHappened();
        }
    }
}
