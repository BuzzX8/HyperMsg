using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg
{
    public class MessageReceiverTests
    {
        private readonly Queue<DeserializationResult<Guid>> deserializationResultsQueue;
        private readonly Queue<Memory<byte>> bufferQueue;
        private readonly Queue<int> readingResultsQueue;
        private readonly Memory<byte> buffer;

        const int GuidSize = 16;

        public MessageReceiverTests()
        {
            deserializationResultsQueue = new Queue<DeserializationResult<Guid>>();
            bufferQueue = new Queue<Memory<byte>>();
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

        [Fact]
        public async Task ReceiveAsync_Returns_Deserialized_Messages_Received_In_Chunks()
        {
            var message1 = Guid.NewGuid();
            var message2 = Guid.NewGuid();

            var messageChunk = new Memory<byte>(new byte[GuidSize + (GuidSize / 2)]);
            message1.ToByteArray().CopyTo(messageChunk);
            message2.ToByteArray().AsMemory(0, GuidSize / 2).CopyTo(messageChunk.Slice(GuidSize));
            bufferQueue.Enqueue(messageChunk);

            messageChunk = new Memory<byte>(new byte [GuidSize / 2]);
            message2.ToByteArray().AsMemory(GuidSize / 2).CopyTo(messageChunk);
            bufferQueue.Enqueue(messageChunk);

            var receiver = new MessageReceiver<Guid>(DeserializeGuid, buffer, ReadBufferFromQueue);

            var actualMessage = await receiver.ReceiveAsync(CancellationToken.None);
            Assert.Equal(message1, actualMessage);

            actualMessage = await receiver.ReceiveAsync(CancellationToken.None);
            Assert.Equal(message2, actualMessage);
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

            if (buffer.Length < GuidSize)
            {
                return new DeserializationResult<Guid>(0, Guid.Empty);
            }

            if (buffer.Length > GuidSize)
            {
                bytes = bytes.Take(GuidSize).ToArray();
            }

            return new DeserializationResult<Guid>(bytes.Length, new Guid(bytes));
        }

        private DeserializationResult<Guid> GetDeserializationResultFromQueue(ReadOnlySequence<byte> buffer) => deserializationResultsQueue.Dequeue();

        private Task<int> GetReadingResultFromQueue(Memory<byte> buffer, CancellationToken token) => Task.FromResult(readingResultsQueue.Dequeue());

        private Task<int> ReadBufferFromQueue(Memory<byte> buffer, CancellationToken token)
        {
            var messagePart = bufferQueue.Dequeue();
            messagePart.CopyTo(buffer);
            return Task.FromResult(messagePart.Length);
        }
    }
}
