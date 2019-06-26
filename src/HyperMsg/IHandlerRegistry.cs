namespace HyperMsg
{
    public interface IHandlerRegistry
    {
        void Register<T>(IMessageHandler<T> handler);
    }
}
