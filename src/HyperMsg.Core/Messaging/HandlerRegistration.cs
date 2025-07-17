namespace HyperMsg.Messaging;

internal class HandlerRegistration : IDisposable
{
    private readonly Action unregAction;

    public HandlerRegistration(Action unregAction) => this.unregAction = unregAction;

    public void Dispose() => unregAction.Invoke();
}
