namespace HyperMsg
{
    public interface IMessageHandlerRegistry<T>
    {
        void Register(AsyncHandler<T> handler);
    }
}