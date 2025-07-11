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
}
