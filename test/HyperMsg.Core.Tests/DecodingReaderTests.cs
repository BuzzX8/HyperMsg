using LanguageExt.Common;
using Xunit;

namespace HyperMsg;

public class DecodingReaderTests
{
    [Fact]
    public void New_Creates_DecodingPipeline()
    {
        var expected = Guid.NewGuid();

        var reader = DecodingReader.New(
            b =>
            {
                var buffer = new byte[16];
                b[..buffer.Length].CopyTo(buffer);
                return new Result<(Guid, int)>((new Guid(buffer), 16));
            },
            () => new Result<Memory<byte>>(expected.ToByteArray()));

        var actual = reader();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task NewAsync_Creates_DecodingPipeline()
    {
        var expected = Guid.NewGuid();

        var reader = DecodingReader.NewAsync(
            b =>
            {
                var buffer = new byte[16];
                b[..buffer.Length].CopyTo(buffer);
                return new Result<(Guid, int)>((new Guid(buffer), 16));
            },
            _ => ValueTask.FromResult(new Result<Memory<byte>>(expected.ToByteArray())));


        var actual = await reader(default);

        Assert.Equal(expected, actual);
    }
}
