namespace HyperMsg
{
    internal class Transmit
    {
        public Transmit(object message) => Message = message;

        internal object Message { get; }
    }
}