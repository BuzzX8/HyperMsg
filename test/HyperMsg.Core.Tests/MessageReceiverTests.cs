using System;
using System.Buffers;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg.Transciever
{
    public class MessageReceiverTests
    {
        private readonly Queue<DeserializationResult<Guid>> deserializationResultsQueue;
        private readonly Queue<int> readingResultsQueue;
        private readonly Memory<byte> buffer;

        const int GuidSize = 16;

        public MessageReceiverTests()
        {
            deserializationResultsQueue = new Queue<DeserializationResult<Guid>>();
            readingResultsQueue = new Queue<int>();
            buffer = new Memory<byte>(new byte[100]);
        }

        [Fact]
        public async Task ReceiveAsync_Returns_Deserialized_Message()
        {
            var expectedMessage = Guid.NewGuid();
            EnqueueDeserializationAndReadResult(expectedMessage, GuidSize, GuidSize);
            var receiver = new MessageReceiver<Guid>(GetDeserializationResultFromQueue, buffer, GetReadingResultFromQueue);

            var acctualMessage = await receiver.ReceiveAsync(CancellationToken.None);

            Assert.Equal(expectedMessage, acctualMessage);
        }

        [Fact]        
        public async Task ReceiveAsync_Returns_Deserialized_Message_When_Message_Deserialized_Not_Immidietly()
        {
            var expectedMessage = Guid.NewGuid();
            EnqueueDeserializationAndReadResult(Guid.Empty, 0, GuidSize / 2);
            EnqueueDeserializationAndReadResult(expectedMessage, GuidSize, GuidSize / 2);

            var receiver = new MessageReceiver<Guid>(GetDeserializationResultFromQueue, buffer, GetReadingResultFromQueue);

            var acctualMessage = await receiver.ReceiveAsync(CancellationToken.None);

            Assert.Equal(expectedMessage, acctualMessage);
        }

        private void EnqueueDeserializationAndReadResult(Guid message, int bytesConsumed, int bytesReaded)
        {
            EnqueueDeserializationResult(message, bytesConsumed);
            EnqueueReadResult(bytesReaded);
        }

        private void EnqueueReadResult(int bytesReaded) => readingResultsQueue.Enqueue(bytesReaded);

        private void EnqueueDeserializationResult(Guid message, int bytesConsumed) => deserializationResultsQueue.Enqueue(new DeserializationResult<Guid>(bytesConsumed, message));

        private DeserializationResult<Guid> DeserializeGuid(ReadOnlySequence<byte> buffer)
        {
            var bytes = buffer.ToArray();

            if (buffer.Length < 16)
            {
                return new DeserializationResult<Guid>(0, Guid.Empty);
            }

            return new DeserializationResult<Guid>(bytes.Length, new Guid(bytes));
        }

        private DeserializationResult<Guid> GetDeserializationResultFromQueue(ReadOnlySequence<byte> buffer) => deserializationResultsQueue.Dequeue();

        private Task<int> GetReadingResultFromQueue(Memory<byte> buffer, CancellationToken token) => Task.FromResult(readingResultsQueue.Dequeue());
    }
}
