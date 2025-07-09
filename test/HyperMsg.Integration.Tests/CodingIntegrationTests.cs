using HyperMsg.Coding;

namespace HyperMsg.Integration.Tests;

public class CodingIntegrationTests : IntegrationTestsBase
{
    public CodingIntegrationTests() : base((_, services) => services.AddCodingContext(EncodeGuid, DecodeGuid))
    {
    }

    [Fact]
    public void CodingContext_ProvidesEncoderAndDecoder()
    {
        var codingContext = GetRequiredService<ICodingContext<Guid>>();
        var buffer = new byte[100];
        var message = Guid.NewGuid();

        // Encode the message
        var bytesEncoded = codingContext.Encoder.Invoke(buffer, message);
        Assert.Equal(16u, bytesEncoded);

        // Decode the message
        var decodingResult = codingContext.Decoder.Invoke(buffer);
        Assert.Equal(message, decodingResult.Message);
        Assert.Equal(16UL, decodingResult.BytesDecoded);
    }

    private static ulong EncodeGuid(Memory<byte> buffer, Guid message)
    {
        var bytes = message.ToByteArray();
        bytes.CopyTo(buffer.Span);
        return (ulong)bytes.Length;
    }

    private static DecodingResult<Guid> DecodeGuid(ReadOnlyMemory<byte> buffer)
    {
        return new(new(buffer.Span[..16]), 16);
    }
}
