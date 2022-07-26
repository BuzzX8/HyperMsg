namespace HyperMsg;

public class Context : IContext
{
    public Context() => (Sender, Receiver) = (new MessageBroker(), new MessageBroker());

    public IBroker Sender { get; }

    public IBroker Receiver { get; }
}
