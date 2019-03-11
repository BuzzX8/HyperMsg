using FakeItEasy;
using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg.Transciever
{
    public class MessageBufferTests
    {
        private IMemoryOwner<byte> memoryOwner;
        private ISender<ReadOnlySequence<byte>> sender;
        private MessageBuffer<Guid> messageBuffer;
        private Memory<byte> buffer;
        private SerializeAction<Guid> serializeAction;

        public MessageBufferTests()
        {
            buffer = new byte[100];
            memoryOwner = A.Fake<IMemoryOwner<byte>>();
            A.CallTo(() => memoryOwner.Memory).Returns(buffer);
            sender = A.Fake<ISender<ReadOnlySequence<byte>>>();
            serializeAction = A.Fake<SerializeAction<Guid>>();
            messageBuffer = new MessageBuffer<Guid>(memoryOwner, sender, serializeAction);
        }

        [Fact]
        public void Write_Invokes_Serialize_Action()
        {
            var message = Guid.NewGuid();

            messageBuffer.Write(message);

            A.CallTo(() => serializeAction.Invoke(A<IBufferWriter<byte>>._, message)).MustHaveHappened();
        }

        [Fact]
        public async Task FlushAsync_Invokes_FlushAsync_For_Writer()
        {
            var message = Guid.NewGuid();
            var actualMessage = Guid.Empty;

            A.CallTo(() => serializeAction.Invoke(A<IBufferWriter<byte>>._, A<Guid>._)).Invokes(foc =>
            {
                var writer = foc.GetArgument<IBufferWriter<byte>>(0);
                var bytes = message.ToByteArray();
                var buffer = writer.GetSpan(bytes.Length);
                bytes.CopyTo(buffer);
                writer.Advance(bytes.Length);
            });

            A.CallTo(() => sender.SendAsync(A<ReadOnlySequence<byte>>._, A<CancellationToken>._)).Invokes(foc =>
            {
                var buffer = foc.GetArgument<ReadOnlySequence<byte>>(0);
                actualMessage = new Guid(buffer.ToArray());
            });

            messageBuffer.Write(message);

            await messageBuffer.FlushAsync();

            Assert.Equal(message, actualMessage);
        }
    }
}
