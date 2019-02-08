using FakeItEasy;
using System;
using System.Buffers;
using Xunit;

namespace HyperMsg.Transciever
{
    public class PipeWriterTests
    {
        private readonly IMemoryOwner<byte> memoryOwner;
        private readonly ReadBufferAction readBufferAction;
        private readonly PipeWriter writer;

        public PipeWriterTests()
        {
            memoryOwner = A.Fake<IMemoryOwner<byte>>();
            readBufferAction = A.Fake<ReadBufferAction>();
            writer = new PipeWriter(memoryOwner, readBufferAction);
        }

        [Fact]
        public void GetMemory_Throws_Exception_Greater_Then_Available_Memory()
        {
            var buffer = Guid.NewGuid().ToByteArray();
            A.CallTo(() => memoryOwner.Memory).Returns(buffer);

            Assert.Throws<ArgumentOutOfRangeException>(() => writer.GetMemory(buffer.Length + 1));
        }

        [Fact]
        public void Flush_Calls_BufferReader()
        {
            writer.Flush();

            A.CallTo(() => readBufferAction.Invoke(A<ReadOnlySequence<byte>>._)).MustHaveHappened();
        }
    }
}
