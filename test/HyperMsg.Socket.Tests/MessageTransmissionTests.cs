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

        services.AddCodingService(DecodeGuid, encoder);
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

        transportTopic.DispatchConnectionRequest(endPoint);
        syncEvent.Wait(TimeSpan.FromSeconds(10));

        Assert.NotNull(acceptedSocket);

        var receivedMessage = new byte[16];
        var receiveTask = acceptedSocket.ReceiveAsync(receivedMessage, SocketFlags.None);
        receiveTask.Wait(TimeSpan.FromSeconds(10));

        Assert.Equal(message.ToByteArray(), receivedMessage);
    }

    [Fact]
    public void Dispatching_ReceiveInBuffer_Receives_Message()
    {
        var message = Guid.NewGuid();
        var receivedMessage = Guid.Empty;
        var acceptedSocket = default(System.Net.Sockets.Socket);
        var syncEvent = new ManualResetEventSlim();

        listeningSocket.Bind(endPoint);
        listeningSocket.Listen();
        transportTopic.Register<ConnectResult>(r =>
        {
            acceptedSocket = listeningSocket.Accept();            
            syncEvent.Set();
        });
        messageTopic.Register<Guid>(m =>
        {
            receivedMessage = m;
            syncEvent.Set();
        });

        transportTopic.DispatchConnectionRequest(endPoint);
        syncEvent.Wait(TimeSpan.FromSeconds(10));
        syncEvent.Reset();

        Assert.NotNull(acceptedSocket);

        acceptedSocket.Send(message.ToByteArray());
        transportTopic.Dispatch(new ReceiveInBuffer());
        syncEvent.Wait(TimeSpan.FromSeconds(10));

        Assert.Equal(message, receivedMessage);
    }

    private static void DecodeGuid(IBufferReader reader, IDispatcher dispatcher)
    {
        var span = reader.GetSpan();
        var message = new Guid(span);
        dispatcher.Dispatch(message);
    }

    public void Dispose()
    {
        provider.Dispose();
        listeningSocket.Dispose();
    }
}
