namespace HyperMsg
{
    public interface IBroker : IForwarder
    {
        IRegistry Registry { get; }
    }
}
