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

        ReadOnlySequence<byte> IBufferReader<byte>.Read() => new ReadOnlySequence<byte>(CommitedMemory);

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
            if (Memory.Length < sizeHint || sizeHint < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (length == 0)
            {
                return Memory;
            }

            var freeMemPos = position + length;

            if (sizeHint > AvailableMemory - freeMemPos)
            {
                CommitedMemory.CopyTo(Memory);
                position = 0;
            }

            return Memory.Slice(length);
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