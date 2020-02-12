using System;

namespace HyperMsg
{
    internal class BufferContext : IBufferContext, IDisposable
    {
        private readonly Buffer receivingBuffer;
        private readonly Buffer transmittingBuffer;

        internal BufferContext(Buffer receivingBuffer, Buffer transmittingBuffer)
        {
            this.receivingBuffer = receivingBuffer;
            this.transmittingBuffer = transmittingBuffer;
        }

        public IBuffer ReceivingBuffer => receivingBuffer;

        public IBuffer TransmittingBuffer => transmittingBuffer;

        public void Dispose()
        {
            receivingBuffer.Dispose();
            transmittingBuffer.Dispose();
        }
    }
}
