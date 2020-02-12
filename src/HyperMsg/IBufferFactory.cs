namespace HyperMsg
{
    public interface IBufferFactory
    {
        IBuffer CreateBuffer(int bufferSize);

        IBufferContext CreateContext(int receivingBufferSize, int transmittingBufferSize);
    }
}
