namespace HyperMsg
{
    internal struct Receive<T>
    {
        internal Receive(T message) => Message = message;
        
        internal T Message { get; }
    }
}
