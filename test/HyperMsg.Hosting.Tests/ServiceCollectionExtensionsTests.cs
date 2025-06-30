using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HyperMsg.Hosting.Tests;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddMessagingWorker_ShouldRegisterServices()
    {        
        // Arrange
        var services = new ServiceCollection();
        
        // Act
        services.AddMessagingWorker();

        // Assert
        using var serviceProvider = services.BuildServiceProvider();
        var worker = serviceProvider.GetService<IEnumerable<IHostedService>>()!.OfType<MessagingWorker>().Single();
        
        Assert.NotNull(worker);
    }
}
