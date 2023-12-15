using LanguageExt.Common;
using Xunit;

namespace HyperMsg;

public class DecodingPipelineTests
{
    [Fact]
    public void New_Creates_DecodingPipeline()
    {
        var expected = Guid.NewGuid();

        var pipeline = DecodingPipeline.New(
            b =>
            {
                var buffer = new byte[16];
                b[..buffer.Length].CopyTo(buffer);
                return new Result<(Guid, int)>((new Guid(buffer), 16));
            },
            () => new Result<Memory<byte>>(expected.ToByteArray()));

        var actual = pipeline();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task NewAsync_Creates_DecodingPipeline()
    {
        var expected = Guid.NewGuid();

        var pipeline = DecodingPipeline.NewAsync(
            b =>
            {
                var buffer = new byte[16];
                b[..buffer.Length].CopyTo(buffer);
                return new Result<(Guid, int)>((new Guid(buffer), 16));
            },
            _ => ValueTask.FromResult(new Result<Memory<byte>>(expected.ToByteArray())));


        var actual = await pipeline(default);

        Assert.Equal(expected, actual);
    }
}
