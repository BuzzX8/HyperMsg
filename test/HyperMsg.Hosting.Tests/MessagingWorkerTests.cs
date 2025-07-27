using FakeItEasy;
using HyperMsg.Messaging;
using Microsoft.Extensions.Logging;

namespace HyperMsg.Hosting.Tests;

public class MessagingWorkerTests
{
    private readonly MessageBroker _broker;
    private readonly List<IMessagingComponent> _components;
    private readonly MessagingWorker _worker;
    private readonly ILogger<MessagingWorker> _logger = A.Fake<ILogger<MessagingWorker>>();

    public MessagingWorkerTests()
    {
        _broker = new();
        _components = [];
        _worker = new MessagingWorker(_broker, _components, _logger);
    }

    [Fact]
    public async Task ExecuteAsync_AttachesAndDetachesComponents()
    {
        // Arrange
        var componentFake = A.Fake<IMessagingComponent>();
        _components.Add(componentFake);

        // Act
        await _worker.StartAsync(default);

        // Assert
        A.CallTo(() => componentFake.Attach(_broker)).MustHaveHappened();
        A.CallTo(() => componentFake.Detach(_broker)).MustNotHaveHappened();
    }

    [Fact]
    public async Task StopAsync_UnregistersComponents()
    {
        // Arrange
        var componentFake = A.Fake<IMessagingComponent>();
        _components.Add(componentFake);
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(100); // Cancel quickly to exit ExecuteAsync
        // Act
        await _worker.StartAsync(cts.Token);
        await _worker.StopAsync(cts.Token);
        // Assert
        A.CallTo(() => componentFake.Detach(_broker)).MustHaveHappened();
    }
}
