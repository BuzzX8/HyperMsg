using Xunit;

namespace HyperMsg.Coding;

public class EncodingWriterTests
{
    [Fact]
    public void New_Creates_Encoding_Writer()
    {
        var buffer = new byte[1024];
        var expectedMessage = Guid.NewGuid();

        var writer = EncodingWriter.New<Guid>(
            (buffer, message) =>
            {
                message.ToByteArray().CopyTo(buffer);
                return message.ToByteArray().Length;
            }, buffer);

        var bytesWritten = writer.Invoke(expectedMessage);

        Assert.Equal(bytesWritten, expectedMessage.ToByteArray().Length);
        Assert.Equal(expectedMessage.ToByteArray(), buffer[..(int)bytesWritten]);
    }
}
