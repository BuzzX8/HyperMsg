using LanguageExt;
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
                return Fin<DecodingResult<Guid>>.Succ(new DecodingResult<Guid>(new Guid(buffer), 16));
            },
            expected.ToByteArray());

        var result = reader();

        Assert.True(result.IsSucc);
        Assert.Equal(expected, result.Case);
    }
}
