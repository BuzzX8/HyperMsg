using FakeItEasy;
using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg
{
    public class PipeReaderWorkItemTests
    {
        [Fact]
        public void Provides_Received_Buffer_To_Buffer_Reader()
        {
            var expected = Guid.NewGuid().ToByteArray();
            var actual = (byte[])null;
            var reader = A.Fake<IPipeReader>();
            A.CallTo(() => reader.ReadAsync(A<CancellationToken>._)).Returns(Task.FromResult(new ReadOnlySequence<byte>(expected)));
            var workItem = new PipeReaderWorkItem(reader, b =>
            {
                actual = b.First.ToArray();
                return 0;
            });

            workItem.ReadPipeAsync().GetAwaiter().GetResult();

            Assert.Equal(expected, actual);            
        }

        [Fact]
        public void Advances_Pipe_Reader_If_Byte_Reader_Returned_Value_Greater_Than_Zero()
        {
            var expected = Guid.NewGuid().ToByteArray();            
            var reader = A.Fake<IPipeReader>();
            A.CallTo(() => reader.ReadAsync(A<CancellationToken>._)).Returns(Task.FromResult(new ReadOnlySequence<byte>(expected)));
            var workItem = new PipeReaderWorkItem(reader, b => (int)b.Length);

            workItem.ReadPipeAsync().GetAwaiter().GetResult();

            A.CallTo(() => reader.Advance(expected.Length)).MustHaveHappened();
        }
    }
}