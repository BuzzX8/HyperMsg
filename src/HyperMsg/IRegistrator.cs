namespace HyperMsg
{
    public interface IRegistrator
    {
        void Register(IRegistry registry);

        void Unregister(IRegistry registry);
    }
}
