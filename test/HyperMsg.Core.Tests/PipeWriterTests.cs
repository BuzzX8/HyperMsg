using FakeItEasy;
using System;
using System.Buffers;
using Xunit;

namespace HyperMsg.Transciever
{
    public class PipeWriterTests
    {
        [Fact]
        public void Flush_Calls_BufferReader()
        {
            var owner = A.Fake<IMemoryOwner<byte>>();
            var reader = A.Fake<Func<ReadOnlySequence<byte>, int>>();
            var writer = new PipeWriter(owner, reader);

            writer.Flush();

            A.CallTo(() => reader.Invoke(A<ReadOnlySequence<byte>>._)).MustHaveHappened();
        }
    }
}
