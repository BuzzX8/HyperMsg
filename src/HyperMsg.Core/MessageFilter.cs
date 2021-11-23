namespace HyperMsg;

public abstract class MessageFilter : ISender
{
    protected MessageFilter(ISender sender) =>
        Sender = sender ?? throw new ArgumentNullException(nameof(sender));

    protected ISender Sender { get; }

    public virtual void Send<T>(T message)
    {
        if (!HandleMessage(message))
        {
            return;
        }

        Sender.Send(message);
    }

    public virtual Task SendAsync<T>(T message, CancellationToken cancellationToken)
    {
        if (!HandleMessage(message))
        {
            return Task.CompletedTask;
        }

        return Sender.SendAsync(message, cancellationToken);
    }

    protected abstract bool HandleMessage<T>(T message);

    protected virtual Task<bool> HandleMessageAsync<T>(T message, CancellationToken _) => Task.FromResult(HandleMessage(message));
}
