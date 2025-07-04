using System.Buffers;

namespace HyperMsg.Buffers;

/// <summary>
/// Provides an implementation of <see cref="IBuffer"/>, <see cref="IBufferReader"/>, and <see cref="IBufferWriter"/> interfaces,
/// enabling efficient reading and writing operations over a contiguous region of memory.
/// </summary>
public sealed class Buffer(Memory<byte> memory) : IBuffer, IBufferReader, IBufferWriter
{
    private readonly Memory<byte> memory = memory;

    private int position;
    private int length;

    /// <summary>
    /// Gets the underlying memory buffer.
    /// </summary>
    private Memory<byte> Memory => memory;

    /// <inheritdoc/>
    public IBufferReader Reader => this;

    /// <inheritdoc/>
    public IBufferWriter Writer => this;

    /// <summary>
    /// Gets the committed portion of the memory buffer.
    /// </summary>
    private Memory<byte> CommitedMemory => Memory.Slice(position, length);

    /// <summary>
    /// Gets the amount of available memory for writing.
    /// </summary>
    private long AvailableMemory => Memory.Length - length;

    #region IBufferReader

    /// <summary>
    /// Advances the read position by the specified count.
    /// </summary>
    /// <param name="count">The number of bytes to advance.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="count"/> is negative or greater than the current length.
    /// </exception>
    void IBufferReader.Advance(int count)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(count);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(count, length);

        position += count;
        length -= count;
    }

    /// <summary>
    /// Gets a <see cref="Memory{T}"/> representing the committed memory available for reading.
    /// </summary>
    /// <returns>The committed memory segment.</returns>
    Memory<byte> IBufferReader.GetMemory() => CommitedMemory;

    /// <summary>
    /// Gets a <see cref="Span{T}"/> representing the committed memory available for reading.
    /// </summary>
    /// <returns>The committed memory span.</returns>
    Span<byte> IBufferReader.GetSpan() => CommitedMemory.Span;

    #endregion

    #region IBufferWriter

    /// <summary>
    /// Advances the write position by the specified count.
    /// </summary>
    /// <param name="count">The number of bytes to advance.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="count"/> is negative or exceeds available memory.
    /// </exception>
    void IBufferWriter<byte>.Advance(int count)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(count);

        if (count > AvailableMemory || count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }

        length += count;
    }

    /// <summary>
    /// Gets a <see cref="Memory{T}"/> segment for writing, with an optional size hint.
    /// </summary>
    /// <param name="sizeHint">The minimum number of bytes required.</param>
    /// <returns>A memory segment for writing.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="sizeHint"/> is negative.
    /// </exception>
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

    /// <summary>
    /// Gets a <see cref="Span{T}"/> segment for writing, with an optional size hint.
    /// </summary>
    /// <param name="sizeHint">The minimum number of bytes required.</param>
    /// <returns>A span for writing.</returns>
    Span<byte> IBufferWriter<byte>.GetSpan(int sizeHint)
    {
        var writer = (IBufferWriter)this;
        return writer.GetMemory(sizeHint).Span;
    }

    #endregion

    /// <summary>
    /// Clears the buffer, resetting the read and write positions.
    /// </summary>
    public void Clear() => position = length = 0;
}
