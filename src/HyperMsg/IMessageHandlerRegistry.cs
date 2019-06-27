namespace HyperMsg
{
    public interface IMessageHandlerRegistry<T>
    {
        void Register(IMessageHandler<T> handler);
    }
}