using FakeItEasy;
using System.Buffers;

namespace HyperMsg.Buffers;

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
    public void Advance_Advances_System_BufferWriter()
    {
        var reader = A.Fake<IBufferWriter<byte>>();
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
    public void Advance_System_BufferWriter_Throws_Exception_If_Count_Value_Greater_Then_Int32_Max()
    {
        var reader = A.Fake<IBufferWriter>();
        var count = (long)int.MaxValue + 1;

        Assert.Throws<ArgumentOutOfRangeException>(() => reader.Advance(count));
    }
}
