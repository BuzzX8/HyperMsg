namespace HyperMsg
{
    public class Runtime : IRuntime, IDisposable
    {
        public Runtime(int serializationBufferSize) : this(BufferFactory.Shared.CreateBuffer(serializationBufferSize))
        { }

        public Runtime(IBuffer serizlizationBuffer)
        {
            SendingBroker = new MessageBroker();
            ReceivingBroker = new MessageBroker();

            var serializersRegistry = new SerializersRegistry(SendingBroker.Registry, serizlizationBuffer);
            serializersRegistry.BufferUpdated += buffer => SendingBroker.Dispatch(new BufferUpdatedEvent(buffer));

            SerializersRegistry = serializersRegistry;
        }

        public IBroker SendingBroker { get; }

        public IBroker ReceivingBroker { get; }

        public ISerializersRegistry SerializersRegistry { get; }

        public void Dispose() => (SerializersRegistry as IDisposable).Dispose();
    }
}
