namespace HyperMsg.Messaging;

public interface IMessagingContext
{
    IDispatcher Dispatcher { get; }

    IHandlerRegistry HandlerRegistry { get; }
}
