using Microsoft.Extensions.Hosting;

namespace HyperMsg.Messaging.Test;

public class MessagingExtensionsTests
{
    public MessagingExtensionsTests()
    {
        var builder = Host.CreateApplicationBuilder();
        builder.Services.AddMessagingWorker();

        var host = builder.Build();
        host.Run();
    }
}
