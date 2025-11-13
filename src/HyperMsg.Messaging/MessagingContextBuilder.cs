using System.Collections.Generic;

namespace HyperMsg.Messaging;

public class MessagingContextBuilder
{
    private readonly List<MessagingContextConfigurator> _configurators = [];
    private readonly List<IMessagingComponent> _components = [];

    public MessagingContextBuilder AddHandler<T>(MessageHandler<T> handler)
    {
        _configurators.Add(ctx => ctx.HandlerRegistry.RegisterHandler(handler));
        return this;
    }

    public MessagingContextBuilder AddAsyncHandler<T>(AsyncMessageHandler<T> handler)
    {
        _configurators.Add(ctx => ctx.HandlerRegistry.RegisterHandler(handler));
        return this;
    }

    public MessagingContextBuilder AddComponent(IMessagingComponent component)
    {
        _components.Add(component);
        return this;
    }

    public IMessagingContext Build()
    {
        var broker = new MessageBroker();

        foreach (var configurator in _configurators)
            configurator.Invoke(broker);

        foreach (var component in _components)
            component.Attach(broker);

        return broker;
    }
}