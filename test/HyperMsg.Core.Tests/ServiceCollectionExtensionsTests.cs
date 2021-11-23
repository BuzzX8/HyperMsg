using Microsoft.Extensions.DependencyInjection;
using System.Buffers;
using Xunit;

namespace HyperMsg;

public class ServiceCollectionExtensionsTests
{
    private readonly ServiceCollection services = new();

    [Fact]
    public void AddMessageBroker_Adds_MessageBroker()
    {
        services.AddMessageBroker();
        var provider = services.BuildServiceProvider();

        var broker = provider.GetRequiredService<MessageBroker>();

        Assert.NotNull(broker);
    }

    [Fact]
    public void AddSharedMemoryPool_Adds_Memory_Pool()
    {
        services.AddSharedMemoryPool();
        var provider = services.BuildServiceProvider();

        var pool = provider.GetService<MemoryPool<byte>>();

        Assert.NotNull(pool);
    }

    [Fact]
    public void AddBufferContext_Adds_BufferContext()
    {
        services.AddSharedMemoryPool()
            .AddBufferContext()
            .AddMessageBroker();
        var provider = services.BuildServiceProvider();

        var context = provider.GetService<IBufferContext>();

        Assert.NotNull(context);
    }

    [Fact]
    public void AddBufferFactory_Adds_BufferFactory()
    {
        services.AddSharedMemoryPool();
        services.AddBufferFactory();
        var provider = services.BuildServiceProvider();

        var factory = provider.GetService<IBufferFactory>();

        Assert.NotNull(factory);
    }
}
