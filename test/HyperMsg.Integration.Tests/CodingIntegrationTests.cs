using HyperMsg.Coding;

namespace HyperMsg.Integration.Tests;

public class CodingIntegrationTests : IntegrationTestsBase
{
    public CodingIntegrationTests() : base((_, services) => services.AddCodingContext(EncodeGuid, DecodeGuid))
    {
    }

    private static ulong EncodeGuid(Memory<byte> buffer, Guid message)
    {
        var bytes = message.ToByteArray();
        bytes.CopyTo(buffer.Span);
        return (ulong)bytes.Length;
    }

    private static DecodingResult<Guid> DecodeGuid(ReadOnlyMemory<byte> buffer)
    {
        return new(new(buffer.Span), 16);
    }
}
