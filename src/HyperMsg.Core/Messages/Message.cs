namespace HyperMsg.Messages
{
    internal struct Message<T>
    {
        internal Message(T data, object id = null) => (Data, Id) = (data, id);

        public object Id { get; }

        public T Data { get; }
    }
}