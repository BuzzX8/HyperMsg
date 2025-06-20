using Microsoft.Extensions.Hosting;

namespace HyperMsg.Hosting.Tests;

public class MessagingExtensionsTests
{
    public MessagingExtensionsTests()
    {
    }

    [Fact]
    public void AddMessagingWorker_ShouldRegisterServices()
    {
        // Arrange
        var builder = Host.CreateApplicationBuilder();
        
        // Act
        var host = builder.Build();

        // Assert
        //host.StartAsync();
        Assert.Fail("This test is not implemented yet. Please implement the test logic to verify service registration.");
    }
}
