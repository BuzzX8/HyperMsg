namespace HyperMsg
{
    public class TransportEventArgs
    {
        public TransportEventArgs(TransportEvent @event)
        {
            Event = @event;
        }

        public TransportEvent Event { get; }
    }
}
