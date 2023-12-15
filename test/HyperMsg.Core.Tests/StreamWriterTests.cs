using Xunit;

namespace HyperMsg;

public class StreamWriterTests
{
    [Fact]
    public void New_Creates_StreamWriter()
    {
        var expected = Guid.NewGuid().ToByteArray();
        var buffer = new byte[1000];

        var stream = new MemoryStream();
        var writer = StreamWriter.New(stream);

        var actual = writer(expected);

        Assert.True(actual.IsSuccess);
        Assert.Equal(expected, stream.ToArray());
    }

    [Fact]
    public async Task NewAsync_Creates_StreamWriter()
    {
        var expected = Guid.NewGuid().ToByteArray();
        var buffer = new byte[1000];

        var stream = new MemoryStream();
        var writer = StreamWriter.NewAsync(stream);

        var actual = await writer(expected, CancellationToken.None);

        Assert.True(actual.IsSuccess);
        Assert.Equal(expected, stream.ToArray());
    }
}
