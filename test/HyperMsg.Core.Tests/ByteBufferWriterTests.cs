using FakeItEasy;
using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg
{
    public class ByteBufferWriterTests
    {        
        private readonly Memory<byte> memory;
        private readonly IMemoryOwner<byte> memoryOwner;
        private readonly AsyncHandler<Memory<byte>> flushHandler;
        private readonly ByteBufferWriter writer;

        public ByteBufferWriterTests()
        {            
            memory = new Memory<byte>(Guid.NewGuid().ToByteArray());
            memoryOwner = A.Fake<IMemoryOwner<byte>>();
            A.CallTo(() => memoryOwner.Memory).Returns(memory);
            flushHandler = A.Fake<AsyncHandler<Memory<byte>>>();
            writer = new ByteBufferWriter(memoryOwner, flushHandler);
        }

        [Fact]
        public void Advance_Throws_Exception_If_Count_Greater_Then_AvailableMemory()
        {
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
        public async Task FlushAsync_Invokes_AsyncHandler_With_Correct_Memory_Slice()
        {
            var token = new CancellationToken();
            var expectedSlice = Guid.NewGuid().ToByteArray();
            var actualSlice = default(byte[]);
            A.CallTo(() => flushHandler.Invoke(A<Memory<byte>>._, token)).Invokes(foc =>
            {
                actualSlice = foc.GetArgument<Memory<byte>>(0).ToArray();
            });

            var buffer = writer.GetMemory(expectedSlice.Length);
            expectedSlice.CopyTo(buffer);
            writer.Advance(expectedSlice.Length);
            await writer.FlushAsync(token);

            Assert.Equal(expectedSlice, actualSlice);
        }

        [Fact]
        public async Task FlushAsync_Frees_Memory()
        {
            var count = 10;
            writer.Advance(count);
            Assert.Equal(writer.CommitedMemory.Length, count);

            await writer.FlushAsync(CancellationToken.None);

            Assert.Equal(0, writer.CommitedMemory.Length);
            Assert.Equal(memory.Length, writer.AvailableMemory);
        }
    }
}