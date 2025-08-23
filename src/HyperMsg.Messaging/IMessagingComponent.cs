namespace HyperMsg.Messaging;

/// <summary>
/// 
/// </summary>
public interface IMessagingComponent
{
    void Attach(IMessagingContext messagingContext);

    void Detach(IMessagingContext messagingContext);
}
