using System;
using System.Buffers;

namespace HyperMsg.Messages
{
    internal struct ReadFromBufferCommand
    {
        public ReadFromBufferCommand(BufferType bufferType, Func<ReadOnlySequence<byte>, int> bufferReader)
        {
            BufferType = bufferType;
            BufferReader = bufferReader;
        }

        public BufferType BufferType { get; }

        public Func<ReadOnlySequence<byte>, int> BufferReader { get; }
    }
}