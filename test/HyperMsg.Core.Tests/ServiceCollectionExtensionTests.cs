using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HyperMsg;

public class ServiceCollectionExtensionTests
{
    private readonly ServiceCollection services = new();

    [Fact]
    public void AddPipeline_Adds_SendingPipeline_Service()
    {
        services.AddCompositeSerializer();
        services.AddDeserializer((_, _) => { });
        services.AddPipeline();

        var provider = services.BuildServiceProvider();
        var pipeline = provider.GetService<Pipeline>();

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
}
