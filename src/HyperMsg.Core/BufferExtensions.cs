using System.Buffers;

namespace HyperMsg;

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

    public static void ForEachSegment(this ReadOnlySequence<byte> data, Action<ReadOnlyMemory<byte>> dataSegmentHandler)
    {
        if (data.Length == 0)
        {
            return;
        }

        if (data.IsSingleSegment)
        {
            dataSegmentHandler(data.First);
        }
        else
        {
            var enumerator = data.GetEnumerator();

            while (enumerator.MoveNext())
            {
                dataSegmentHandler(enumerator.Current);
            }
        }
    }

    public static async Task ForEachSegment(this ReadOnlySequence<byte> data, AsyncAction<ReadOnlyMemory<byte>> dataSegmentHandler, CancellationToken cancellationToken = default)
    {
        if (data.Length == 0)
        {
            return;
        }

        if (data.IsSingleSegment)
        {
            await dataSegmentHandler(data.First, cancellationToken);
        }
        else
        {
            var enumerator = data.GetEnumerator();

            while (enumerator.MoveNext())
            {
                await dataSegmentHandler(enumerator.Current, cancellationToken);
            }
        }
    }

    public static void ForEachSegment(this IBufferReader bufferReader, Action<ReadOnlyMemory<byte>> dataSegmentHandler, bool advanceReader = true)
    {
        var data = bufferReader.Read();
        data.ForEachSegment(dataSegmentHandler);

        if (advanceReader)
        {
            bufferReader.Advance(data.Length);
        }
    }

    public static async Task ForEachSegment(this IBufferReader bufferReader, AsyncAction<ReadOnlyMemory<byte>> dataSegmentHandler, bool advanceReader = true, CancellationToken cancellationToken = default)
    {
        var data = bufferReader.Read();
        await data.ForEachSegment(dataSegmentHandler, cancellationToken);

        if (advanceReader)
        {
            bufferReader.Advance(data.Length);
        }
    }
}
