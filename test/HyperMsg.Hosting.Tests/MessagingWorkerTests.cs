using FakeItEasy;
using HyperMsg.Messaging;

namespace HyperMsg.Hosting.Tests;

public class MessagingWorkerTests
{
    private readonly MessageBroker _broker;
    private readonly List<IMessagingComponent> _components;
    private readonly MessagingWorker _worker;

    public MessagingWorkerTests()
    {
        _broker = new();
        _components = [];
        _worker = new MessagingWorker(_broker, _components);
    }

    [Fact]
    public async Task ExecuteAsync_AttachesAndDetachesComponents()
    {
        // Arrange
        var componentFake = A.Fake<IMessagingComponent>();
        _components.Add(componentFake);

        var worker = new MessagingWorker(_broker, _components);

        using var cts = new CancellationTokenSource();
        cts.CancelAfter(100); // Cancel quickly to exit ExecuteAsync

        // Act
        await worker.StartAsync(cts.Token);

        // Assert
        A.CallTo(() => componentFake.Attach(_broker)).MustHaveHappened();
        A.CallTo(() => componentFake.Detach(_broker)).MustNotHaveHappened();
    }
}
