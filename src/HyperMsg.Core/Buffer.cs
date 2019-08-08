using System;
using System.Buffers;

namespace HyperMsg
{
    public class Buffer : IBuffer
    {
        public IBufferReader Reader => throw new NotImplementedException();

        public IBufferWriter<byte> Writer => throw new NotImplementedException();

        public event AsyncAction<IBuffer> Flushed;
    }
}