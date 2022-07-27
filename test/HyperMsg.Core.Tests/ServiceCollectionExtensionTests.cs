using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HyperMsg;

public class ServiceCollectionExtensionTests
{
    private readonly ServiceCollection services = new();

    [Fact]
    public void AddSendBufferFilter_Adds_SendBufferFilter_Service()
    {
        services.AddCompositeSerializer();
        services.AddSendingPipeline();

        var provider = services.BuildServiceProvider();
        var pipeline = provider.GetService<SendingPipeline>();

        Assert.NotNull(pipeline);
    }

    [Fact]
    public void AddCompositeSerializer_Adds_SerializationFilter_Service()
    {
        services.AddCompositeSerializer();

        var provider = services.BuildServiceProvider();
        var instance = provider.GetService<CompositeSerializer>();
        var @interface = provider.GetService<ISerializer>();

        Assert.NotNull(instance);
        Assert.NotNull(@interface);
    }
}
