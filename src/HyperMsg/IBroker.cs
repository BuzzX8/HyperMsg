namespace HyperMsg
{
    public interface IBroker : IForwarder
    {
        void Register(IRegistrator registrator);
    }
}
