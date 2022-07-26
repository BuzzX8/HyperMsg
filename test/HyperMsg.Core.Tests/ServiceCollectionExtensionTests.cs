using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HyperMsg;

public class ServiceCollectionExtensionTests
{
    private readonly ServiceCollection services = new();

    [Fact]
    public void AddContext_Adds_Context_Service()
    {
        services.AddContext();

        var provider = services.BuildServiceProvider();
        var context = provider.GetService<IContext>();

        Assert.NotNull(context);
        Assert.NotNull(context.Sender);
        Assert.NotNull(context.Receiver);
    }

    [Fact]
    public void AddSendBufferFilter_Adds_SendBufferFilter_Service()
    {
        services.AddSendBufferFilter();

        var provider = services.BuildServiceProvider();
        var filter = provider.GetService<SendBufferFilter>();

        Assert.NotNull(filter);
    }

    [Fact]
    public void AddSerializationFilter_Adds_SerializationFilter_Service()
    {
        services.AddContext();
        services.AddSerializationFilter();

        var provider = services.BuildServiceProvider();
        var filter = provider.GetService<SerializationFilter>();

        Assert.NotNull(filter);
    }
}
