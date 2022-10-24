using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HyperMsg;

public class ServiceCollectionExtensionTests
{
    private readonly ServiceCollection services = new();

    [Fact]
    public void AddPipeline_Adds_SendingPipeline_Service()
    {        
        services.AddDeserializer((_, _) => { });
        services.AddPipeline();

        var provider = services.BuildServiceProvider();
        var pipeline = provider.GetService<Pipeline>();

        Assert.NotNull(pipeline);
    }
}
