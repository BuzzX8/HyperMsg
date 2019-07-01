using FakeItEasy;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg
{
    public class MessageBufferObserverTests
    {
        private readonly DeserializeFunc<Guid> deserializeFunc;
        private readonly IBufferReader bufferReader;
        private readonly MessageBufferObserver<Guid> observer;

        public MessageBufferObserverTests()
        {
            deserializeFunc = A.Fake<DeserializeFunc<Guid>>();
            bufferReader = A.Fake<IBufferReader>();
            observer = new MessageBufferObserver<Guid>(deserializeFunc, bufferReader);
        }

        [Fact]
        public async Task CheckBufferAsync_Provides_Buffer_Received_From_BufferReader_To_DeserializeFunc()
        {
            var buffer = new ReadOnlySequence<byte>(Guid.NewGuid().ToByteArray());
            var token = new CancellationToken();
            A.CallTo(() => bufferReader.ReadAsync(token)).Returns(Task.FromResult(buffer));

            await observer.CheckBufferAsync(token);

            A.CallTo(() => deserializeFunc.Invoke(buffer)).MustHaveHappened();
        }

        [Fact]
        public async Task CheckBufferAsync_Advances_Buffer_With_Correct_MessageSize()
        {
            var messageSize = 100;
            A.CallTo(() => deserializeFunc.Invoke(A<ReadOnlySequence<byte>>._)).Returns(new DeserializationResult<Guid>(messageSize, Guid.Empty));

            await observer.CheckBufferAsync(CancellationToken.None);

            A.CallTo(() => bufferReader.Advance(messageSize)).MustHaveHappened();
        }

        [Fact]
        public async Task CheckBufferAsync_Does_Not_Rises_MessageDeserialized_If_DeserializetionFunc_Returns_Zero_Result()
        {
            bool eventRaised = false;
            observer.MessageDeserialized += m => eventRaised = true;
            A.CallTo(() => deserializeFunc.Invoke(A<ReadOnlySequence<byte>>._)).Returns(new DeserializationResult<Guid>(0, Guid.Empty));

            await observer.CheckBufferAsync(CancellationToken.None);

            Assert.False(eventRaised);
        }

        [Fact]
        public async Task CheckBufferAsync_Rises_MessageDeserialized_If_DeserializationFunc_Returns_Non_Zero_Result()
        {
            var expectedMessage = Guid.NewGuid();
            var actualMessage = Guid.Empty;
            observer.MessageDeserialized += m => actualMessage = m;
            A.CallTo(() => deserializeFunc.Invoke(A<ReadOnlySequence<byte>>._)).Returns(new DeserializationResult<Guid>(16, expectedMessage));

            await observer.CheckBufferAsync(CancellationToken.None);

            Assert.Equal(expectedMessage, actualMessage);
        }

        [Fact]
        public async Task CheckBufferAsync_Rises_Message_Deserialized_While_DeserializationFunc_Returns_Non_Zero_Result()
        {
            var results = new[]
            {
                new DeserializationResult<Guid>(1, Guid.NewGuid()),
                new DeserializationResult<Guid>(2, Guid.NewGuid()),
                new DeserializationResult<Guid>(0, Guid.Empty)
            };
            var resultQueue = new Queue<DeserializationResult<Guid>>(results);
            var expectedMessages = results.Take(results.Length - 1).Select(r => r.Message);
            var actualMessages = new List<Guid>();
            observer.MessageDeserialized += m => actualMessages.Add(m);
            A.CallTo(() => deserializeFunc.Invoke(A<ReadOnlySequence<byte>>._)).Returns(resultQueue.Dequeue());

            await observer.CheckBufferAsync(CancellationToken.None);

            Assert.Equal(expectedMessages, actualMessages);
        }
    }
}
