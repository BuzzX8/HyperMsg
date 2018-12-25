using FakeItEasy;
using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg
{
    public class ReadPipeActionTests
    {
        [Fact]
        public async Task InvokeAsync_Provides_Buffer_To_BufferReader()
        {
            var reader = A.Fake<IPipeReader>();
            var data = Guid.NewGuid().ToByteArray();
            A.CallTo(() => reader.ReadAsync(A<CancellationToken>._)).Returns(Task.FromResult(new ReadOnlySequence<byte>(data)));
            var actualData = (byte[])null;
            var action = new ReadPipeAction(reader, b =>
            {
                actualData = b.First.ToArray();
                return (int)b.Length;
            });

            await action.InvokeAsync();

            Assert.Equal(data, actualData);
        }

        [Fact]
        public async Task InvokeAsync_Advances_Reader()
        {
            var reader = A.Fake<IPipeReader>();
            var length = 8;
            var action = new ReadPipeAction(reader, b => length);

            await action.InvokeAsync();

            A.CallTo(() => reader.Advance(length)).MustHaveHappened();
        }
    }
}
