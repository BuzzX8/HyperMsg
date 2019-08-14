using FakeItEasy;
using System;
using System.Buffers;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg
{
    public class BufferTests
    {
        private readonly IMemoryOwner<byte> memoryOwner;
        private readonly Memory<byte> memory;
        private readonly Buffer buffer;

        const int MemorySize = 4096;

        public BufferTests()
        {
            memoryOwner = A.Fake<IMemoryOwner<byte>>();
            memory = new byte[MemorySize];
            A.CallTo(() => memoryOwner.Memory).Returns(memory);
            buffer = new Buffer(memoryOwner);
        }

        [Fact]
        public async Task ReadAsync_Returns_Written_Bytes()
        {
            var expectedBytes = Guid.NewGuid().ToByteArray();
            WriteBytes(expectedBytes);

            var actualBytes = await buffer.Reader.ReadAsync(default);

            Assert.Equal(expectedBytes, actualBytes.ToArray());
        }

        [Fact]
        public async Task Reader_Advance_Advances_Reading_Position()
        {
            var bytes = Guid.NewGuid().ToByteArray();
            var advanceCount = bytes.Length / 2;
            var expectedBytes = bytes.Skip(advanceCount).ToArray();
            WriteBytes(bytes);

            buffer.Reader.Advance(advanceCount);
            var actualBytes = await buffer.Reader.ReadAsync(default);

            Assert.Equal(expectedBytes, actualBytes.ToArray());
        }

        [Fact]
        public void Writer_Advance_Throws_Exception_If_Count_Greater_Then_AvailableMemory()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.Writer.Advance(MemorySize + 1));
        }

        [Fact]
        public void Writer_GetMemory_Throws_Exception_Greater_Then_Available_Memory()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.Writer.GetMemory(memory.Length + 1));
        }

        [Fact]
        public void Writer_FlushAsync_Rises_Flushed_Event()
        {
            var wasRised = false;
            buffer.FlushRequested += (b, t) =>
            {
                wasRised = true;
                Assert.Same(buffer, b);
                return Task.CompletedTask;
            };

            Assert.True(wasRised);
        }

        [Fact]
        public void Dispose_Disposes_MemoryOwner()
        {
            buffer.Dispose();

            A.CallTo(() => memoryOwner.Dispose()).MustHaveHappened();
        }

        private void WriteBytes(byte[] bytes)
        {
            var memory = buffer.Writer.GetMemory(bytes.Length);
            bytes.CopyTo(memory);
            buffer.Writer.Advance(bytes.Length);
        }
    }
}