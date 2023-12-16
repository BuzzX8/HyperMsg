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
                b.CopyTo(buffer);
                return new Result<(Guid, int)>((new Guid(buffer), 16));
            },
            b =>
            {
                expected.ToByteArray().CopyTo(b);
                return new Result<int>(expected.ToByteArray().Length);
            },
            expected.ToByteArray());

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
                b.CopyTo(buffer);
                return new Result<(Guid, int)>((new Guid(buffer), 16));
            },
            (b, t) =>
            {
                expected.ToByteArray().CopyTo(b);
                return ValueTask.FromResult(new Result<int>(expected.ToByteArray().Length));
            },
            expected.ToByteArray());


        var actual = await reader(default);

        Assert.Equal(expected, actual);
    }
}
