using Xunit;

namespace HyperMsg.Messaging;

public class MessagingContextExtensionsTests
{
    private readonly MessageBroker broker = new();

    [Fact]
    public void DispatchRequest_Should_Return_Response_From_Registered_Handler()
    {
        // Arrange
        var request = Guid.NewGuid();
        var expectedResponse = request.ToString();
        broker.RegisterRequestHandler<Guid, string>(req => req.ToString());
        // Act
        var response = broker.DispatchRequest<Guid, string>(request);
        // Assert
        Assert.Equal(expectedResponse, response);
    }

    [Fact]
    public async Task DispatchRequestAsync_Should_Return_Response_From_Registered_Handler()
    {
        // Arrange
        var request = Guid.NewGuid();
        var expectedResponse = request.ToString();
        broker.RegisterRequestHandler<Guid, string>(async (req, _) => await Task.FromResult(req.ToString()));
        
        // Act
        var response = await broker.DispatchRequestAsync<Guid, string>(request);
        
        // Assert
        Assert.Equal(expectedResponse, response);
    }

    [Fact]
    public void RegisterHandler_Should_Register_Handler()
    {
        // Arrange
        var message = "test";
        var wasCalled = false;
        broker.RegisterHandler<string>(_ => wasCalled = true);

        // Act
        broker.Dispatch(message);

        // Assert
        Assert.True(wasCalled);
    }

    [Fact]
    public async Task RegisterHandler_Should_Register_Async_Handler()
    {
        // Arrange
        var message = "test";
        var wasCalled = false;
        broker.RegisterHandler<string>(async (_, _) =>
        {
            wasCalled = true;
            await Task.CompletedTask;
        });

        // Act
        await broker.DispatchAsync(message);

        // Assert
        Assert.True(wasCalled);
    }

    [Fact]
    public void Dispose_Should_Unregister_All_Handlers()
    {
        // Arrange
        var message = "test";
        var wasCalled = false;
        var registration = broker.RegisterHandler<string>(_ => wasCalled = true);
        // Act
        registration.Dispose();
        broker.Dispatch(message);
        // Assert
        Assert.False(wasCalled);
    }
}
