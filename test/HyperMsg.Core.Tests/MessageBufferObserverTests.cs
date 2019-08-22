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
        private readonly IBufferReader<byte> bufferReader;
        private readonly MessageBufferObserver<Guid> observer;
                
        private readonly Stack<DeserializationResult<Guid>> deserializationResults;
        private readonly ReadOnlySequence<byte> buffer;

        public MessageBufferObserverTests()
        {
            deserializeFunc = A.Fake<DeserializeFunc<Guid>>();
            bufferReader = A.Fake<IBufferReader<byte>>();
            observer = new MessageBufferObserver<Guid>(deserializeFunc);

            deserializationResults = new Stack<DeserializationResult<Guid>>();
            buffer = new ReadOnlySequence<byte>(Enumerable.Range(0, 100).Select(i => (byte)i).ToArray());

            PushEmptyDeserializationResult();
            A.CallTo(() => bufferReader.Read()).Returns(buffer);
            A.CallTo(() => deserializeFunc.Invoke(A<ReadOnlySequence<byte>>._)).ReturnsLazily(foc => deserializationResults.Pop());
        }

        [Fact]
        public async Task CheckBufferAsync_Provides_Buffer_Received_From_BufferReader_To_DeserializeFunc()
        {
            var buffer = new ReadOnlySequence<byte>(Guid.NewGuid().ToByteArray());
            var token = new CancellationToken();
            A.CallTo(() => bufferReader.Read()).Returns(buffer);

            await observer.CheckBufferAsync(bufferReader, token);

            A.CallTo(() => deserializeFunc.Invoke(buffer)).MustHaveHappened();
        }

        [Fact]
        public async Task CheckBufferAsync_Advances_Buffer_With_Correct_MessageSize()
        {
            var messageSize = 100;            
            PushDeserializationResult(messageSize, Guid.Empty);

            await observer.CheckBufferAsync(bufferReader, CancellationToken.None);

            A.CallTo(() => bufferReader.Advance(messageSize)).MustHaveHappened();
        }

        [Fact]
        public async Task CheckBufferAsync_Does_Not_Rises_MessageDeserialized_If_DeserializetionFunc_Returns_Zero_Result()
        {
            bool eventRaised = false;
            observer.MessageDeserialized += (m, t) => { eventRaised = true; return Task.CompletedTask; };

            await observer.CheckBufferAsync(bufferReader, CancellationToken.None);

            Assert.False(eventRaised);
        }

        [Fact]
        public async Task CheckBufferAsync_Rises_MessageDeserialized_If_DeserializationFunc_Returns_Non_Zero_Result()
        {
            var expectedMessage = Guid.NewGuid();
            var actualMessage = Guid.Empty;
            observer.MessageDeserialized += (m, t) => { actualMessage = m; return Task.CompletedTask; };
            PushDeserializationResult(16, expectedMessage);

            await observer.CheckBufferAsync(bufferReader, CancellationToken.None);

            Assert.Equal(expectedMessage, actualMessage);
        }

        [Fact]
        public async Task CheckBufferAsync_Rises_Message_Deserialized_While_DeserializationFunc_Returns_Non_Zero_Result()
        {
            PushDeserializationResult(1, Guid.NewGuid());
            PushDeserializationResult(2, Guid.NewGuid());
            var expectedMessages = deserializationResults.Take(deserializationResults.Count - 1).Select(r => r.Message).ToArray();
            var actualMessages = new List<Guid>();
            observer.MessageDeserialized += (m, t) => { actualMessages.Add(m); return Task.CompletedTask; };

            await observer.CheckBufferAsync(bufferReader, CancellationToken.None);

            Assert.Equal(expectedMessages, actualMessages);
        }

        [Fact]
        public async Task CheckBufferAsync_Corrrectly_Slices_Buffer()
        {
            var messageSize = 20;
            PushDeserializationResult(messageSize, Guid.Empty);

            await observer.CheckBufferAsync(bufferReader, CancellationToken.None);

            A.CallTo(() => deserializeFunc.Invoke(buffer.Slice(messageSize))).MustHaveHappened();
        }

        [Fact]
        public async Task CheckBufferAsync_Throws_Exception_If_Deserializer_Returns_Incorrect_Result()
        {
            PushDeserializationResult((int)buffer.Length + 1, Guid.Empty);

            await Assert.ThrowsAsync<DeserializationException>(() => observer.CheckBufferAsync(bufferReader, CancellationToken.None));
        }

        private void PushDeserializationResult(int messageSize, Guid message) => deserializationResults.Push(new DeserializationResult<Guid>(messageSize, message));

        private void PushEmptyDeserializationResult() => deserializationResults.Push(new DeserializationResult<Guid>(0, Guid.Empty));
    }
}
