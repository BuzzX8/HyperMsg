using FakeItEasy;
using System;
using System.Buffers;
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