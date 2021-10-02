using System;

namespace HyperMsg
{
    internal class BufferContext : IBufferContext, IDisposable
    {
        private readonly BufferEventProxy receivingBuffer;
        private readonly BufferEventProxy transmittingBuffer;

        internal BufferContext(Buffer receivingBuffer, Buffer transmittingBuffer, ISender sender)
        {
            this.receivingBuffer = new BufferEventProxy(BufferType.Receive, receivingBuffer, sender);
            this.transmittingBuffer = new BufferEventProxy(BufferType.Transmit, transmittingBuffer, sender);
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
