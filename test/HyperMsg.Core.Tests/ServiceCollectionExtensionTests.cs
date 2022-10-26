using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HyperMsg;

public class ServiceCollectionExtensionTests
{
    private readonly ServiceCollection services = new();

    [Fact]
    public void AddKernel_Adds_Dispatcher_Registry_And_TransportGateway_Service()
    {
        var serializer = A.Fake<ISerializer>();
        var deserializer = A.Fake<Deserializer>();
        services.AddKernel(deserializer, serializer);

        var provider = services.BuildServiceProvider();
        var dispatcher = provider.GetService<IDispatcher>();
        var registry = provider.GetService<IRegistry>();
        var transportGateway = provider.GetService<ITransportGateway>();

        Assert.NotNull(dispatcher);
        Assert.NotNull(registry);
        Assert.NotNull(transportGateway);
    }
}
