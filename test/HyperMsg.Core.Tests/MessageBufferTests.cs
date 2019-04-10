using FakeItEasy;
using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg
{
    public class MessageBufferTests
    {
        private readonly SerializeAction<Guid> serializeAction;
        private readonly Memory<byte> buffer;
        private readonly Func<Memory<byte>, CancellationToken, Task> writeAsync;
        private readonly MessageBuffer<Guid> messageBuffer;

        public MessageBufferTests()
        {
            serializeAction = A.Fake<SerializeAction<Guid>>();
            buffer = new byte[100];
            writeAsync = A.Fake<Func<Memory<byte>, CancellationToken, Task>>();
            messageBuffer = new MessageBuffer<Guid>(serializeAction, buffer, writeAsync);
        }

        [Fact]
        public async Task FlushAsync_Provides_Buffer_Content_To_Write_Action()
        {
            await messageBuffer.FlushAsync();

            A.CallTo(() => writeAsync.Invoke(A<Memory<byte>>._, A<CancellationToken>._)).MustHaveHappened();
        }

        [Fact]
        public async Task SendAsync_Serializes_And_Sends_Message()
        {
            var message = Guid.NewGuid();

            await messageBuffer.SendAsync(message);

            A.CallTo(() => writeAsync.Invoke(A<Memory<byte>>._, A<CancellationToken>._)).MustHaveHappened();
            A.CallTo(() => serializeAction.Invoke(A<IBufferWriter<byte>>._, message)).MustHaveHappened();
        }

        [Fact]
        public void Write_Calls_Serialize_Action_For_Message()
        {
            var message = Guid.NewGuid();

            messageBuffer.Write(message);

            A.CallTo(() => serializeAction.Invoke(A<IBufferWriter<byte>>._, message)).MustHaveHappened();
        }
    }
}
