using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HyperMsg;

public class ServiceCollectionExtensionTests
{
    private readonly ServiceCollection services = new();

    [Fact]
    public void AddCodingService_Adds_CodingService_And_CodingGateway_Services()
    {
        var serializer = A.Fake<IEncoder>();
        var deserializer = A.Fake<Decoder>();
        services.AddCodingService(deserializer, serializer);

        var provider = services.BuildServiceProvider();
        var service = provider.GetService<CodingService>();
        var transportGateway = provider.GetService<ICoderGateway>();

        Assert.NotNull(service);
        Assert.NotNull(transportGateway);
    }
}
