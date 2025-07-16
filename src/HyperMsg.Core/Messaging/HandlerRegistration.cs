namespace HyperMsg.Messaging;

internal class HandlerRegistration : IDisposable
{
    private readonly Delegate handler;
    private readonly IHandlerRegistry registry;

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}
