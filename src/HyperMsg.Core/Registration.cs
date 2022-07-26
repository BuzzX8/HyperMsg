namespace HyperMsg;

internal class Registration<T> : IDisposable
{
    private readonly IRegistry registry;
    private readonly Action<T> handler;

    public Registration(IRegistry registry, Action<T> handler)
    {
        this.registry = registry;
        this.handler = handler;
    }

    internal Action<T> Handler => handler;

    public void Dispose() => registry.Deregister(handler);
}
