using LanguageExt.Common;
using Xunit;

namespace HyperMsg;

public class DecodingPipelineTests
{
    [Fact]
    public void New_Creates_DecodingPipeline()
    {
        var buffer = new byte[1024];
        var expected = Guid.NewGuid();

        var pipeline = DecodingPipeline.New(
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
            buffer);

        var actual = pipeline();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task NewAsync_Creates_DecodingPipeline()
    {
        var buffer = new byte[1024];
        var expected = Guid.NewGuid();

        var pipeline = DecodingPipeline.NewAsync(
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
            buffer);


        var actual = await pipeline(default);

        Assert.Equal(expected, actual);
    }
}
