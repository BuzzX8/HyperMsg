using FakeItEasy;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg
{
    public class MessageReceiverTests
    {
        private readonly Queue<DeserializationResult<Guid>> deserializationResultsQueue;
        private readonly Queue<ReadOnlySequence<byte>> bufferQueue;
        private readonly DeserializeFunc<Guid> deserialize;
        private readonly IBufferReader bufferReader;
        private readonly MessageReceiver<Guid> receiver;

        const int GuidSize = 16;

        public MessageReceiverTests()
        {
            deserializationResultsQueue = new Queue<DeserializationResult<Guid>>();
            bufferQueue = new Queue<ReadOnlySequence<byte>>();
            deserialize = A.Fake<DeserializeFunc<Guid>>();
            bufferReader = A.Fake<IBufferReader>();
            receiver = new MessageReceiver<Guid>(deserialize, bufferReader);            
        }

        [Fact]
        public async Task ReceiveAsync_Returns_Deserialized_Message()
        {
            var expectedMessage = Guid.NewGuid();
            A.CallTo(() => deserialize.Invoke(A<ReadOnlySequence<byte>>._)).Returns(new DeserializationResult<Guid>(GuidSize, expectedMessage));

            var acctualMessage = await receiver.ReceiveAsync(CancellationToken.None);

            Assert.Equal(expectedMessage, acctualMessage);
            A.CallTo(() => bufferReader.Advance(GuidSize)).MustHaveHappened();
        }

        [Fact]
        public async Task ReceiveAsync_Privides_Buffer_To_Deserializer_Received_From_Reader()
        {
            var buffer = new ReadOnlySequence<byte>(Guid.NewGuid().ToByteArray());
            A.CallTo(() => deserialize.Invoke(A<ReadOnlySequence<byte>>._)).Returns(new DeserializationResult<Guid>(GuidSize, Guid.Empty));
            A.CallTo(() => bufferReader.ReadAsync(A<CancellationToken>._)).Returns(Task.FromResult(buffer));

            await receiver.ReceiveAsync(CancellationToken.None);

            A.CallTo(() => deserialize.Invoke(buffer)).MustHaveHappened();
        }
    }
}
