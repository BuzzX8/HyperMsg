using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HyperMsg.Socket.Tests;

public class ServiceCollectionExtensionTests
{
    private readonly ServiceCollection services = new();

    [Fact]
    public void AddSocketService_Adds_Required_Services()
    {
        var coderGateway = A.Fake<ICoderGateway>();

        services.AddSingleton(coderGateway);
        services.AddSocketService();
        var provider = services.BuildServiceProvider();

        var addedServices = provider.GetServices<IHostedService>();

        Assert.NotNull(addedServices);
        Assert.Contains(addedServices, s => s is ConnectionService);
        Assert.Contains(addedServices, s => s is TransmissionService);
        Assert.Contains(addedServices, s => s is SocketService);
    }
}
