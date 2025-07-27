using HyperMsg.Hosting;

namespace HyperMsg.Integration.Tests;

public class MessagingWorkerIntegrationTests : IntegrationTestsBase
{
    public MessagingWorkerIntegrationTests() : base((_, services) => services.AddMessagingWorker())
    {
    }
}
