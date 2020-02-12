namespace HyperMsg
{
    /// <summary>
    /// Represents factory for creating buffers and buffer context.
    /// </summary>
    public interface IBufferFactory
    {
        /// <summary>
        /// Creates buffer.
        /// </summary>
        /// <param name="bufferSize">Size of buffer.</param>
        /// <returns></returns>
        IBuffer CreateBuffer(int bufferSize);

        /// <summary>
        /// Creates buffer context.
        /// </summary>
        /// <param name="receivingBufferSize">Size of receiving buffer.</param>
        /// <param name="transmittingBufferSize">Size of transmitting buffer.</param>
        /// <returns></returns>
        IBufferContext CreateContext(int receivingBufferSize, int transmittingBufferSize);
    }
}
