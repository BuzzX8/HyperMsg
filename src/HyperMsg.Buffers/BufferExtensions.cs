using System.Buffers;

namespace HyperMsg.Buffers;

public static class BufferExtensions
{
    public static void Advance(this IBufferReader reader, long count)
    {
        VerifyCountParam(count);
        reader.Advance((int)count);
    }

    public static void Advance(this IBufferWriter writer, long count)
    {
        VerifyCountParam(count);
        writer.Advance((int)count);
    }

    public static void Advance(this IBufferWriter<byte> writer, long count)
    {
        VerifyCountParam(count);
        writer.Advance((int)count);
    }

    private static void VerifyCountParam(long count)
    {
        if (count > int.MaxValue)
        {
            throw new ArgumentOutOfRangeException($"Value of count should be lesser or equal to {int.MaxValue}");
        }
    }

    public static void Write(this IBufferWriter writer, ReadOnlySpan<byte> value)
    {
        var span = writer.GetSpan(value.Length);

        if (value.TryCopyTo(span))
        {
            writer.Advance(value.Length);
        }
        else
        {
            throw new InvalidOperationException("Can not copy value into buffer.");
        }
    }

    public static bool TryWrite(this IBufferWriter writer, ReadOnlySpan<byte> value)
    {
        var span = writer.GetSpan(value.Length);

        if (value.TryCopyTo(span))
        {
            writer.Advance(value.Length);
            return true;
        }

        return false;
    }
}
