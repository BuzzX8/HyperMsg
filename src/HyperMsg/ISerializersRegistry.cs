namespace HyperMsg
{
    public interface ISerializersRegistry
    {
        void Register<T>(Action<IBufferWriter, T> serializer);

        void Deregister<T>();
    }
}
