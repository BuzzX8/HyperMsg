using HyperMsg.Transport;
using HyperMsg.Transport.Sockets;
using System.Net;

namespace HyperMsg.Integration.Tests;

public class SocketTransportIntegrationTests : IntegrationTestsBase
{
    private static EndPoint EndPoint => new IPEndPoint(IPAddress.Loopback, 8088);

    public SocketTransportIntegrationTests() : base((_, services) => services.AddClientSocketTransport(EndPoint))
    {
    }

    [Fact]
    public void SocketTransport_Should_Resolve()
    {
        // Arrange & Act
        var transport = GetRequiredService<ITransportContext>();
        // Assert
        Assert.NotNull(transport);
        Assert.IsType<SocketTransport>(transport);
    }
}
