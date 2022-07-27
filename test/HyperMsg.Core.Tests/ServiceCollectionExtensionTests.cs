using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HyperMsg;

public class ServiceCollectionExtensionTests
{
    private readonly ServiceCollection services = new();

    [Fact]
    public void AddSendingPipeline_Adds_SendingPipeline_Service()
    {
        services.AddCompositeSerializer();
        services.AddSendingPipeline();

        var provider = services.BuildServiceProvider();
        var pipeline = provider.GetService<SendingPipeline>();

        Assert.NotNull(pipeline);
    }

    [Fact]
    public void AddCompositeSerializer_Adds_CompositeSerializer_Service()
    {
        services.AddCompositeSerializer();

        var provider = services.BuildServiceProvider();
        var instance = provider.GetService<CompositeSerializer>();
        var @interface = provider.GetService<ISerializer>();

        Assert.NotNull(instance);
        Assert.NotNull(@interface);
    }

    [Fact]
    public void AddReceivingPipeline_Adds_ReceivingPipeline_Service()
    {
        services.AddMessageBroker();
        services.AddReceivingPipeline();

        var provider = services.BuildServiceProvider();
        var pipeline = provider.GetRequiredService<ReceivingPipeline>();

        Assert.NotNull(pipeline);
    }
}
