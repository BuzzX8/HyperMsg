﻿using System.Buffers;

namespace HyperMsg;

/// <summary>
/// Provides implementation for buffer interfaces
/// </summary>
public sealed class Buffer : IBuffer, IBufferReader, IBufferWriter, IDisposable
{
    private readonly IMemoryOwner<byte> memoryOwner;
    private readonly object sync;

    private int position;
    private int length;

    public Buffer(IMemoryOwner<byte> memoryOwner)
    {
        this.memoryOwner = memoryOwner ?? throw new ArgumentNullException(nameof(memoryOwner));
        sync = new();
    }

    private Memory<byte> Memory => memoryOwner.Memory;

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

        lock (sync)
        {
            if (count > length)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            position += count;
            length -= count;
        }
    }

    ReadOnlyMemory<byte> IBufferReader.GetMemory() => CommitedMemory;

    ReadOnlySpan<byte> IBufferReader.GetSpan() => CommitedMemory.Span;

    #endregion

    #region IBufferWriter

    void IBufferWriter<byte>.Advance(int count)
    {
        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }

        lock (sync)
        {
            if (count > AvailableMemory || count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            length += count;
        }
    }

    Memory<byte> IBufferWriter<byte>.GetMemory(int sizeHint)
    {
        if (sizeHint < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(sizeHint));
        }

        lock (sync)
        {
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
    }

    Span<byte> IBufferWriter<byte>.GetSpan(int sizeHint)
    {
        var writer = (IBufferWriter)this;
        return writer.GetMemory(sizeHint).Span;
    }

    #endregion

    public void Clear() => position = length = 0;

    public void Dispose() => memoryOwner.Dispose();
}
