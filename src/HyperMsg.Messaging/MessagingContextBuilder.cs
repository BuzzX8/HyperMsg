using Microsoft.Extensions.DependencyInjection;

namespace HyperMsg.Messaging;

public class MessagingContextBuilder
{
    private readonly IServiceCollection services;

    internal MessagingContextBuilder(IServiceCollection services) => this.services = services;

    public IServiceCollection Services => services;

    public MessagingContextBuilder AddHandler<T>(MessageHandler<T> handler)
    {
        return this;
    }

    public MessagingContextBuilder AddAsyncHandler<T>(AsyncMessageHandler<T> handler)
    {
        return this;
    }

    public MessagingContextBuilder AddComponent(IMessagingComponent component)
    {
        return this;
    }
}