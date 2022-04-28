namespace HyperMsg
{
    public interface IRuntime
    {
        IBroker Sender { get; }

        IBroker Receiver { get; }

        IServiceProvider ServiceProvider { get; }
    }
}
