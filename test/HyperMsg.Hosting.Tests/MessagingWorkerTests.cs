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
        //A.CallTo(() => componentFake.Detach(_broker)).MustHaveHappened();
    }

    //[Fact]
    //public async Task RegisterHandlersAsync_RegistersHandlers()
    //{
    //    // Arrange
    //    var called = false;
    //    var workerFake = A.Fake<MessagingWorker>(
    //        options => options.WithArgumentsForConstructor(() => new MessagingWorker(_broker, _components))
    //                          .CallsBaseMethods());

    //    A.CallTo(() => workerFake.RegisterHandlersAsync(A<IMessagingContext>._, A<CancellationToken>._))
    //        .Invokes(() => called = true)
    //        .Returns(Task.CompletedTask);

    //    // Act
    //    await workerFake.StartAsync(CancellationToken.None);

    //    // Assert
    //    Assert.True(called);
    //}

    //[Fact]
    //public async Task UnregisterHandlersAsync_UnregistersHandlers()
    //{
    //    // Arrange
    //    var called = false;
    //    var workerFake = A.Fake<MessagingWorker>(
    //        options => options.WithArgumentsForConstructor(() => new MessagingWorker(_broker, _components))
    //                          .CallsBaseMethods());

    //    A.CallTo(() => workerFake.UnregisterHandlersAsync(A<IMessagingContext>._, A<CancellationToken>._))
    //        .Invokes(() => called = true)
    //        .Returns(Task.CompletedTask);

    //    // Act
    //    await workerFake.StopAsync(CancellationToken.None);

    //    // Assert
    //    Assert.True(called);
    //}

    //[Fact]
    //public async Task OnHeartBeatAsync_CanBeCalled()
    //{
    //    // Arrange
    //    var called = false;
    //    var workerFake = A.Fake<MessagingWorker>(
    //        options => options.WithArgumentsForConstructor(() => new MessagingWorker(_broker, _components))
    //                          .CallsBaseMethods());

    //    A.CallTo(() => workerFake.OnHeartBeatAsync(A<CancellationToken>._))
    //        .Invokes(() => called = true)
    //        .Returns(Task.CompletedTask);

    //    // Act
    //    await workerFake.StartAsync(CancellationToken.None);

    //    // Assert
    //    Assert.True(called);
    //}
}
