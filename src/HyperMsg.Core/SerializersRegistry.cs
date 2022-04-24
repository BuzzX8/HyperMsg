namespace HyperMsg
{
    public class SerializersRegistry : ISerializersRegistry, IDisposable
    {
        private readonly IRegistry registry;
        private readonly IBuffer buffer;

        private readonly Dictionary<Type, IDisposable> registrations = new();

        public SerializersRegistry(IRegistry registry, IBuffer buffer) =>
            (this.registry, this.buffer) = (registry, buffer);

        public void Register<T>(Action<IBufferWriter, T> serializer)
        {
            Deregister<T>();

            var registration = new Registration<T>(registry, message =>
            {
                serializer.Invoke(buffer.Writer, message);
                BufferUpdated?.Invoke(buffer);
            });
            registrations[typeof(T)] = registration;
            
            registry.Register(registration.Handler);
        }

        public void Deregister<T>()
        {
            if (!registrations.ContainsKey(typeof(T)))
                return;

            registrations[typeof(T)].Dispose();
            registrations.Remove(typeof(T));
        }

        public void Dispose()
        {
            if (buffer is IDisposable disp)
            {
                disp.Dispose();
            }
        }

        public event Action<IBuffer> BufferUpdated;
    }
}
