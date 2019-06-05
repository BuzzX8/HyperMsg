namespace HyperMsg
{
    public interface IHandlerRegistry
    {
        void Register<T>(IHandler<T> handler);
    }
}
