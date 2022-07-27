using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HyperMsg;

public class ServiceCollectionExtensionTests
{
    private readonly ServiceCollection services = new();

    [Fact]
    public void AddSendBufferFilter_Adds_SendBufferFilter_Service()
    {
        services.AddSendBufferFilter();

        var provider = services.BuildServiceProvider();
        var filter = provider.GetService<SendingPipeline>();

        Assert.NotNull(filter);
    }

    [Fact]
    public void AddSerializationFilter_Adds_SerializationFilter_Service()
    {
        services.AddSerializationFilter();

        var provider = services.BuildServiceProvider();
        var filter = provider.GetService<SerializationFilter>();

        Assert.NotNull(filter);
    }
}
