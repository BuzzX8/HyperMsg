using FakeItEasy;
using System;
using System.Buffers;
using Xunit;

namespace HyperMsg.Transciever
{
    public class PipeWriterTests
    {
        private readonly IMemoryOwner<byte> memoryOwner;
        private readonly Memory<byte> memory;
        private readonly ReadBufferAction readBufferAction;
        private readonly PipeWriter writer;

        public PipeWriterTests()
        {
            memoryOwner = A.Fake<IMemoryOwner<byte>>();
            memory = new Memory<byte>(Guid.NewGuid().ToByteArray());            
            readBufferAction = A.Fake<ReadBufferAction>();
            writer = new PipeWriter(memoryOwner, readBufferAction);

            A.CallTo(() => memoryOwner.Memory).Returns(memory);
        }

        [Fact]
        public void Advance_Throws_Exception_If_Count_Greater_Then_AvailableMemory()
        {
            var avaibleMemory = 1024;
            A.CallTo(() => memoryOwner.Memory).Returns(new Memory<byte>(new byte[avaibleMemory]));
            Assert.Throws<ArgumentOutOfRangeException>(() => writer.Advance(writer.AvailableMemory + 1));
        }

        [Fact]
        public void Advance_Reduces_AvailableMemory()
        {
            var count = memory.Length / 2;
            Assert.Equal(memory.Length, writer.AvailableMemory);

            writer.Advance(count);

            Assert.Equal(memory.Length - count, writer.AvailableMemory);
        }

        [Fact]
        public void GetMemory_Throws_Exception_Greater_Then_Available_Memory()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => writer.GetMemory(memory.Length + 1));
        }

        [Fact]
        public void Flush_Calls_BufferReader()
        {
            writer.Flush();

            A.CallTo(() => readBufferAction.Invoke(A<ReadOnlySequence<byte>>._)).MustHaveHappened();
        }
    }
}
