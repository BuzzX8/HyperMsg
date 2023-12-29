using LanguageExt;
using LanguageExt.Common;
using Xunit;

namespace HyperMsg;

public class EncodingWriterTests
{
    [Fact]
    public void New_Creates_Encoding_Writer()
    {
        var buffer = new byte[1024];
        var expected = Guid.NewGuid();

        var writer = EncodingWriter.New<Guid>(
            (buffer, message) =>
            {
                message.ToByteArray().CopyTo(buffer);
                return Fin<int>.Succ(message.ToByteArray().Length);
            }, buffer);

        var result = writer(expected);

        Assert.True(result.IsSucc);
        Assert.Equal(expected.ToByteArray(), buffer[..expected.ToByteArray().Length]);
    }
}
