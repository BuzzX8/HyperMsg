using FakeItEasy;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg.Transciever
{
    public class MessageTransceiverTests
    {
        private readonly IMessageBuffer<Guid> messageBuffer;        
        private readonly ISubject<Guid> subject;
        private readonly MessageReceiverFactory<Guid> messageReceiverFactory;
        private MessageTransceiver<Guid> transceiver;
        private List<Func<IDisposable>> runners;

        public MessageTransceiverTests()
        {
            messageBuffer = A.Fake<IMessageBuffer<Guid>>();
            subject = A.Fake<ISubject<Guid>>();
            messageReceiverFactory = A.Fake<MessageReceiverFactory<Guid>>();
            runners = new List<Func<IDisposable>>(A.CollectionOfFake<Func<IDisposable>>(10));
            transceiver = new MessageTransceiver<Guid>(messageBuffer, messageReceiverFactory, subject, runners);
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

        [Fact]
        public void Subscribe_Calls_MessageSubject_Subscribe()
        {
            var disposable = A.Fake<IDisposable>();
            A.CallTo(() => subject.Subscribe(A<IObserver<Guid>>._)).Returns(disposable);
            var observer = A.Fake<IObserver<Guid>>();

            var actual = transceiver.Subscribe(observer);

            Assert.Same(disposable, actual);
            A.CallTo(() => subject.Subscribe(observer)).MustHaveHappened();
        }

        [Fact]
        public void ReadBuffer_Invokes_ReadBufferAction()
        {
            var message = Guid.NewGuid();
            var onMessage = (Action<Guid>)null;
            A.CallTo(() => messageReceiverFactory.Invoke(A<Action<Guid>>._)).Invokes(foc =>
            {
                onMessage = foc.GetArgument<Action<Guid>>(0);
            })
            .Returns(b =>
            {
                onMessage(message);
                return -1;
            });

            var readed = transceiver.ReadBuffer(new ReadOnlySequence<byte>());

            A.CallTo(() => subject.OnNext(message)).MustHaveHappened();
        }
    }
}