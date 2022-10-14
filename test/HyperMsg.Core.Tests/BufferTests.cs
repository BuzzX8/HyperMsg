using FakeItEasy;
using System.Buffers;
using Xunit;

namespace HyperMsg;

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
    public void GetMemory_Returns_Written_Bytes()
    {
        var expectedBytes = Guid.NewGuid().ToByteArray();
        WriteBytes(expectedBytes);

        var actualBytes = buffer.Reader.GetMemory();

        Assert.Equal(expectedBytes, actualBytes.ToArray());
    }

    [Fact]
    public void GetMemory_Advance_Advances_Reading_Position()
    {
        var bytes = Guid.NewGuid().ToByteArray();
        var advanceCount = bytes.Length / 2;
        var expectedBytes = bytes.Skip(advanceCount).ToArray();
        WriteBytes(bytes);

        buffer.Reader.Advance(advanceCount);
        var actualBytes = buffer.Reader.GetMemory();

        Assert.Equal(expectedBytes, actualBytes.ToArray());
    }

    [Fact]
    public void Writer_Advance_Throws_Exception_If_Count_Greater_Then_AvailableMemory()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => buffer.Writer.Advance(MemorySize + 1));
    }

    [Fact]
    public void Clear_Removes_All_Data_From_Buffer()
    {
        WriteBytes(Guid.NewGuid().ToByteArray());

        buffer.Clear();
        var bytes = buffer.Reader.GetMemory();

        Assert.Equal(0, bytes.Length);
    }

    [Fact]
    public void Dispose_Disposes_MemoryOwner()
    {
        buffer.Dispose();

        A.CallTo(() => memoryOwner.Dispose()).MustHaveHappened();
    }

    [Fact]
    public void GetMemory_Returns_Correct_Memory_Segment_After_Advance()
    {
        var bytes = Guid.NewGuid().ToByteArray();
        WriteBytes(bytes);

        var memorySegment = buffer.Writer.GetMemory().Slice(0, bytes.Length).ToArray();

        Assert.NotEqual(memorySegment, bytes);
    }

    [Fact]
    public void GetMemory_Defrags_Buffer()
    {
        var dataSize = MemorySize - (MemorySize / 4);
        var bytes = Enumerable.Range(0, dataSize).Select(i => Guid.NewGuid().ToByteArray()[0]).ToArray();
        WriteBytes(bytes);
        buffer.Reader.Advance(MemorySize / 4);
        var sizeHint = MemorySize / 2;

        var memory = buffer.Writer.GetMemory(sizeHint);

        Assert.True(memory.Length >= sizeHint);
    }

    [Fact]
    public void Correctly_Advances_Reader()
    {
        var data = Guid.NewGuid().ToByteArray();
        WriteBytes(data);
        buffer.Reader.Advance(data.Length);

        WriteBytes(data);

        Assert.Equal(data, buffer.Reader.GetMemory().ToArray());
    }

    [Fact]
    public void Write_Extension_Writes_Over_Buffer_Size()
    {
        var data = Guid.NewGuid().ToByteArray();
        var iterationCount = (MemorySize / data.Length) + 2;

        for (int i = 0; i < iterationCount; i++)
        {
            buffer.Writer.Write(data);
            buffer.Reader.Advance(data.Length);
        }
    }

    [Fact]
    public void Write_Writes_Over_Buffer_Size_()
    {
        var data = Guid.NewGuid().ToByteArray();

        for (int i = 0; i < MemorySize; i++)
        {
            WriteBytes(data);
            buffer.Reader.Advance(data.Length);
        }
    }

    private void WriteBytes(byte[] bytes)
    {
        var memory = buffer.Writer.GetMemory(bytes.Length);
        bytes.CopyTo(memory);
        buffer.Writer.Advance(bytes.Length);
    }
}
