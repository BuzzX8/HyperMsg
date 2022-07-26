namespace HyperMsg;

public interface IContext
{
    IBroker Sender { get; }

    IBroker Receiver { get; }
}
