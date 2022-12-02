using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net;
using System.Net.Sockets;

namespace HyperMsg.Socket.Tests;

public class MessageTransmissionTests : IDisposable
{
    private readonly ITopic messageTopic;
    private readonly ITopic transportTopic;
    private readonly ServiceCollection services = new();
    private readonly ServiceProvider provider;

    private readonly System.Net.Sockets.Socket listeningSocket;

    private static readonly IPEndPoint endPoint = new(IPAddress.Loopback, 8083);

    public MessageTransmissionTests() 
    {
        var encoder = new CompositeEncoder();
        encoder.Add<Guid>((w, m) =>
        {
            var span = w.GetSpan();
            m.TryWriteBytes(span);
            w.Advance(16);
        });

        transportTopic = new MessageBroker();

        services.AddCodingService(null, encoder);
        services.AddSocketService(transportTopic);

        provider = services.BuildServiceProvider();

        var hostedServices = provider.GetServices<IHostedService>();

        foreach(var service in hostedServices)
            service.StartAsync(default).Wait();

        messageTopic = provider.GetRequiredService<CodingService>();
        listeningSocket = new(SocketType.Stream, ProtocolType.Tcp);
    }

    [Fact]
    public void Dispatching_Message_With_MessageTopic_Transmits_Message()
    {
        var message = Guid.NewGuid();
        var acceptedSocket = default(System.Net.Sockets.Socket);
        var syncEvent = new ManualResetEventSlim();

        listeningSocket.Bind(endPoint);
        listeningSocket.Listen();
        transportTopic.Register<ConnectResult>(r =>
        {
            acceptedSocket = listeningSocket.Accept();
            messageTopic.Dispatch(message);
            syncEvent.Set();
        });

        transportTopic.Dispatch(new Connect(endPoint));
        syncEvent.Wait(TimeSpan.FromSeconds(10));

        Assert.NotNull(acceptedSocket);

        var receivedMessage = new byte[16];
        acceptedSocket.Receive(receivedMessage);

        Assert.Equal(message.ToByteArray(), receivedMessage);
    }

    public void Dispose()
    {
        provider.Dispose();
        listeningSocket.Dispose();
    }
}
