using System.Buffers;

namespace HyperMsg.Buffers;

/// <summary>
/// Provides implementation for buffer interfaces
/// </summary>
public sealed class Buffer : IBufferReader, IBufferWriter
{
    private readonly Memory<byte> memory;

    private int position;
    private int length;

    public Buffer(Memory<byte> memory) => this.memory = memory;

    private Memory<byte> Memory => memory;

    public IBufferReader Reader => this;

    public IBufferWriter Writer => this;

    private Memory<byte> CommitedMemory => Memory.Slice(position, length);

    private long AvailableMemory => Memory.Length - length;

    #region IBufferReader

    void IBufferReader.Advance(int count)
    {
        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }

        if (count > length)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }

        position += count;
        length -= count;
    }

    Memory<byte> IBufferReader.GetMemory() => CommitedMemory;

    Span<byte> IBufferReader.GetSpan() => CommitedMemory.Span;

    #endregion

    #region IBufferWriter

    void IBufferWriter<byte>.Advance(int count)
    {
        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }

        if (count > AvailableMemory || count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }

        length += count;
    }

    Memory<byte> IBufferWriter<byte>.GetMemory(int sizeHint)
    {
        if (sizeHint < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(sizeHint));
        }

        if (length == 0)
        {
            position = 0;
        }

        var freeMemPos = position + length;

        if (sizeHint > AvailableMemory - freeMemPos || sizeHint == 0)
        {
            CommitedMemory.CopyTo(Memory);
            position = 0;
            freeMemPos = length;
        }

        return Memory[freeMemPos..];
    }

    Span<byte> IBufferWriter<byte>.GetSpan(int sizeHint)
    {
        var writer = (IBufferWriter)this;
        return writer.GetMemory(sizeHint).Span;
    }

    #endregion

    public void Clear() => position = length = 0;
}
