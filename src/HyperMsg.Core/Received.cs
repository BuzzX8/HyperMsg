namespace HyperMsg
{
    internal class Received
    {
        internal Received(object message) => Message = message;
        
        internal object Message { get; }
    }
}
