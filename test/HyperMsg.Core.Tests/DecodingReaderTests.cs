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
                return new DecodingResult<Guid>(new Guid(buffer), b.Length);
            },
            expected.ToByteArray());

        var result = reader();

        var (actualMessage, bytesDecoded) = reader.Invoke();

        Assert.Equal(16, bytesDecoded);
        Assert.Equal(expected, actualMessage);
    }
}
