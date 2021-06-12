using FakeItEasy;
using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg
{
    public class BufferExtensionsTests
    {
        [Fact]
        public void Advance_Advances_BufferReader()
        {
            var reader = A.Fake<IBufferReader>();
            var count = (long)Guid.NewGuid().ToByteArray()[0];

            reader.Advance(count);

            A.CallTo(() => reader.Advance((int)count)).MustHaveHappened();
        }

        [Fact]
        public void Advance_BufferReader_Throws_Exception_If_Count_Value_Greater_Then_Int32_Max()
        {
            var reader = A.Fake<IBufferReader>();
            var count = (long)int.MaxValue + 1;

            Assert.Throws<ArgumentOutOfRangeException>(() => reader.Advance(count));
        }

        [Fact]
        public void Advance_Advances_BufferWriter()
        {
            var reader = A.Fake<IBufferWriter>();
            var count = (long)Guid.NewGuid().ToByteArray()[0];

            reader.Advance(count);

            A.CallTo(() => reader.Advance((int)count)).MustHaveHappened();
        }

        [Fact]
        public void Advance_BufferWriter_Throws_Exception_If_Count_Value_Greater_Then_Int32_Max()
        {
            var reader = A.Fake<IBufferWriter>();
            var count = (long)int.MaxValue + 1;

            Assert.Throws<ArgumentOutOfRangeException>(() => reader.Advance(count));
        }

        [Fact]
        public void ForEachSegment_Does_Not_Invokes_Handler_For_Empty_Data()
        {            
            var data = new ReadOnlySequence<byte>();
            var handler = A.Fake<Action<ReadOnlyMemory<byte>>>();

            data.ForEachSegment(handler);

            A.CallTo(() => handler.Invoke(A<ReadOnlyMemory<byte>>._)).MustNotHaveHappened();
        }

        [Fact]
        public async Task ForEachSegment_Does_Not_Invokes_Async_Handler_For_Empty_Data()
        {
            var data = new ReadOnlySequence<byte>();
            var handler = A.Fake<AsyncAction<ReadOnlyMemory<byte>>>();

            await data.ForEachSegment(handler);

            A.CallTo(() => handler.Invoke(A<ReadOnlyMemory<byte>>._, A<CancellationToken>._)).MustNotHaveHappened();
        }

        [Fact]
        public void ForEachSegment_Invokes_Handler_For_Single_Segment()
        {
            var segment = Guid.NewGuid().ToByteArray().AsMemory();
            var data = new ReadOnlySequence<byte>(segment);
            var handler = A.Fake<Action<ReadOnlyMemory<byte>>>();

            data.ForEachSegment(handler);

            A.CallTo(() => handler.Invoke(segment)).MustHaveHappened();
        }

        [Fact]
        public async Task ForEachSegment_Invokes_Async_Handler_For_Single_Segment()
        {
            var segment = Guid.NewGuid().ToByteArray().AsMemory();
            var data = new ReadOnlySequence<byte>(segment);
            var handler = A.Fake<AsyncAction<ReadOnlyMemory<byte>>>();

            await data.ForEachSegment(handler, default);

            A.CallTo(() => handler.Invoke(segment, A<CancellationToken>._)).MustHaveHappened();
        }
    }
}
