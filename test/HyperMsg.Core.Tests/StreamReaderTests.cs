using Xunit;

namespace HyperMsg;

public static class StreamReaderTests
{
    [Fact]
    public static void New_Creates_StreamReader()
    {
        var expected = Guid.NewGuid().ToByteArray();
        var buffer = new byte[1000];

        var stream = new MemoryStream(expected);
        var reader = StreamReader.New(stream);

        var actual = reader(buffer).Match(
            Succ: buffer => buffer,
            Fail: error => throw error);
                
        Assert.Equal(expected, actual.ToArray());
    }

    [Fact]
    public static async Task NewAsync_Creates_StreamReader()
    {
        var expected = Guid.NewGuid().ToByteArray();
        var buffer = new byte[1000];

        var stream = new MemoryStream(expected);
        var reader = StreamReader.NewAsync(stream);

        var actual = (await reader(buffer, CancellationToken.None)).Match(
            Succ: buffer => buffer,
            Fail: error => throw error);
                
        Assert.Equal(expected, actual.ToArray());
    }
}
