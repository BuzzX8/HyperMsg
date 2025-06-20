namespace HyperMsg.Messaging;

/// <summary>
/// 
/// </summary>
public interface IMessagingContext
{
    /// <summary>
    /// 
    /// </summary>
    IDispatcher Dispatcher { get; }

    /// <summary>
    /// 
    /// </summary>
    IHandlerRegistry HandlerRegistry { get; }
}
