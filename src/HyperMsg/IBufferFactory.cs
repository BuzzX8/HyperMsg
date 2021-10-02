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
    }
}
