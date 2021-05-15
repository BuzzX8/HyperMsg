﻿using System;
using System.Buffers;

namespace HyperMsg
{
    /// <summary>
    /// Provides implementation for buffer interfaces
    /// </summary>
    public sealed class Buffer : IBuffer, IBufferReader<byte>, IBufferWriter<byte>, IDisposable
    {
        private readonly IMemoryOwner<byte> memoryOwner;

        private int position;
        private int length;

        public Buffer(IMemoryOwner<byte> memoryOwner)
        {
            this.memoryOwner = memoryOwner ?? throw new ArgumentNullException(nameof(memoryOwner));
        }

        private Memory<byte> Memory => memoryOwner.Memory;

        public IBufferReader<byte> Reader => this;

        public IBufferWriter<byte> Writer => this;

        private Memory<byte> CommitedMemory => Memory.Slice(position, length);

        private int AvailableMemory => Memory.Length - length;

        void IBufferReader<byte>.Advance(int count)
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

        ReadOnlySequence<byte> IBufferReader<byte>.Read() => new(CommitedMemory);

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
            var writer = (IBufferWriter<byte>)this;
            return writer.GetMemory(sizeHint).Span;
        }

        public void Clear() => position = length = 0;

        public void Dispose() => memoryOwner.Dispose();
    }
}