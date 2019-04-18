using FakeItEasy;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using ReadAsyncFunc = System.Func<System.Memory<byte>, System.Threading.CancellationToken, System.Threading.Tasks.Task<int>>;

namespace HyperMsg
{
    public class BufferReaderTests
    {
        private readonly Memory<byte> buffer;
        private readonly ReadAsyncFunc readAsync;
        private readonly BufferReader reader;

        public BufferReaderTests()
        {
            buffer = new Memory<byte>(new byte[100]);
            readAsync = A.Fake<ReadAsyncFunc>();
            reader = new BufferReader(buffer, readAsync);
        }

        [Fact]
        public async Task ReadAsync_Returns_Readed_Bytes()
        {
            var expected = Guid.NewGuid().ToByteArray();
            A.CallTo(() => readAsync.Invoke(A<Memory<byte>>._, A<CancellationToken>._)).Invokes(foc =>
            {
                var buffer = foc.GetArgument<Memory<byte>>(0);
                expected.CopyTo(buffer);
            }).Returns(Task.FromResult(expected.Length));

            var actual = await reader.ReadAsync(CancellationToken.None);

            Assert.Equal(expected, actual.First.ToArray());
        }

        [Fact]
        public async Task Advance_Advances_Reading_Position()
        {
            var bytes = Guid.NewGuid().ToByteArray();            
            A.CallTo(() => readAsync.Invoke(A<Memory<byte>>._, A<CancellationToken>._)).Invokes(foc =>
            {
                var buffer = foc.GetArgument<Memory<byte>>(0);
                bytes.CopyTo(buffer);
            }).Returns(Task.FromResult(bytes.Length));
            var advanceCount = bytes.Length / 2;
            var expected = bytes.Skip(advanceCount).ToArray();

            await reader.ReadAsync(CancellationToken.None);
            reader.Advance(advanceCount);
            var actual = await reader.ReadAsync(CancellationToken.None);

            Assert.Equal(expected, actual.First.ToArray().Take(advanceCount));
        }
    }
}
