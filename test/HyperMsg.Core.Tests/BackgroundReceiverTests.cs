using FakeItEasy;
using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg
{
    public class BackgroundReceiverTests : IDisposable
    {
        private readonly DeserializeFunc<Guid> deserializeFunc;
        private readonly IBufferReader bufferReader;
        private readonly IMessageHandler<Guid> handler;
        private readonly BackgroundReceiver<Guid> backgroundReceiver;

        private readonly ManualResetEventSlim @event = new ManualResetEventSlim();
        private readonly TimeSpan waitTimeout = TimeSpan.FromSeconds(2);

        public BackgroundReceiverTests()
        {
            deserializeFunc = A.Fake<DeserializeFunc<Guid>>();
            bufferReader = A.Fake<IBufferReader>();
            handler = A.Fake<IMessageHandler<Guid>>();
            backgroundReceiver = new BackgroundReceiver<Guid>(deserializeFunc, bufferReader, handler);
        }

        [Fact]
        public void DoWorkIterationAsync_Reads_Buffer_With_BufferReader()
        {
            A.CallTo(() => bufferReader.ReadAsync(A<CancellationToken>._)).Invokes(foc =>
            {
                backgroundReceiver.Handle(TransportEvent.Closed);
                @event.Set();
            })
            .Returns(Task.FromResult(new ReadOnlySequence<byte>()));            

            backgroundReceiver.Handle(TransportEvent.Opened);
            @event.Wait(waitTimeout);

            A.CallTo(() => bufferReader.ReadAsync(A<CancellationToken>._)).MustHaveHappened();
        }

        [Fact]
        public void DoWorkIterationAsync_Invokes_Deserialization_Func()
        {
            var buffer = new ReadOnlySequence<byte>(Guid.NewGuid().ToByteArray());

            A.CallTo(() => bufferReader.ReadAsync(A<CancellationToken>._)).Returns(buffer);
            A.CallTo(() => deserializeFunc.Invoke(buffer)).Invokes(foc =>
            {
                backgroundReceiver.Handle(TransportEvent.Closed);
                @event.Set();
            });

            backgroundReceiver.Handle(TransportEvent.Opened);
            @event.Wait(waitTimeout);

            A.CallTo(() => deserializeFunc.Invoke(buffer)).MustHaveHappened();
        }

        [Fact]
        public void DoWorkIterationAsync_Advances_Buffer_Reader_When_Message_Deserialized()
        {
            var messageSize = 16;

            A.CallTo(() => deserializeFunc.Invoke(A<ReadOnlySequence<byte>>._)).Returns(new DeserializationResult<Guid>(messageSize, Guid.Empty));
            A.CallTo(() => bufferReader.Advance(messageSize)).Invokes(foc =>
            {
                backgroundReceiver.Handle(TransportEvent.Closed);
                @event.Set();                
            });

            backgroundReceiver.Handle(TransportEvent.Opened);
            @event.Wait(waitTimeout);

            A.CallTo(() => bufferReader.Advance(messageSize)).MustHaveHappened();
        }

        [Fact]
        public void DoWorkIterationAsync_Invokes_MessageHandler_When_Message_Deserialized()
        {
            var message = Guid.NewGuid();

            A.CallTo(() => deserializeFunc.Invoke(A<ReadOnlySequence<byte>>._)).Returns(new DeserializationResult<Guid>(16, message));
            A.CallTo(() => handler.HandleAsync(message, A<CancellationToken>._)).Invokes(foc =>
            {
                backgroundReceiver.Handle(TransportEvent.Closed);
                @event.Set();
            }).Returns(Task.CompletedTask);

            backgroundReceiver.Handle(TransportEvent.Opened);
            @event.Wait(waitTimeout);

            A.CallTo(() => handler.HandleAsync(message, A<CancellationToken>._)).MustHaveHappened();
        }

        public void Dispose() => backgroundReceiver.Dispose();
    }
}
