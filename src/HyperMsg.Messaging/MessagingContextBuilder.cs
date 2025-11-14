using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HyperMsg.Messaging;

public class MessagingContextBuilder
{
    private readonly IServiceCollection services;

    internal MessagingContextBuilder(IServiceCollection services) => this.services = services;

    public IServiceCollection Services => services;

    public MessagingContextBuilder AddHandler<T>(MessageHandler<T> handler)
    {
        MessagingContextConfigurator configurator = (context) =>
        {
            context.HandlerRegistry.RegisterHandler(handler);
        };

        services.AddSingleton(configurator);

        return this;
    }

    public MessagingContextBuilder AddAsyncHandler<T>(AsyncMessageHandler<T> handler)
    {
        MessagingContextConfigurator configurator = (context) =>
        {
            context.HandlerRegistry.RegisterHandler(handler);
        };

        services.AddSingleton(configurator);

        return this;
    }

    public MessagingContextBuilder AddComponent(IMessagingComponent component)
    {
        //TODO: Implement AddComponent(IMessagingComponent)
        return this;
    }

    public MessagingContextBuilder AddComponent<T>() where T : IMessagingComponent
    {
        //TODO: Implement AddComponent<T>
        return this;
    }
}

public delegate void MessagingContextConfigurator(IMessagingContext messagingContext);