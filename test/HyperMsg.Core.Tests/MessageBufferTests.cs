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
        private IPipeWriter writer;
        private MessageBuffer<Guid> messageBuffer;
        private Action<IBufferWriter<byte>, Guid> serializeAction;

        public MessageBufferTests()
        {
            writer = A.Fake<IPipeWriter>();
            serializeAction = A.Fake<Action<IBufferWriter<byte>, Guid>>();
            messageBuffer = new MessageBuffer<Guid>(writer, serializeAction);
        }

        [Fact]
        public void Write_Invokes_Serialize_Action()
        {
            var message = Guid.NewGuid();

            messageBuffer.Write(message);

            A.CallTo(() => serializeAction.Invoke(writer, message)).MustHaveHappened();
        }

        [Fact]
        public async Task FlushAsync_Invokes_FlushAsync_For_Writer()
        {
            await messageBuffer.FlushAsync();

            A.CallTo(() => writer.FlushAsync(A<CancellationToken>._)).MustHaveHappened();
        }
    }
}
