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
        private SerializeAction<Guid> serializeAction;

        public MessageBufferTests()
        {
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
            await messageBuffer.FlushAsync();

            //A.CallTo(() => sender.SendAsync(A<ReadOnlySequence<byte>>._, A<CancellationToken>._).MustHaveHappened();
        }
    }
}
