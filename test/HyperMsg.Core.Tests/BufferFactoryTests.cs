using Xunit;

namespace HyperMsg;

public class BufferFactoryTests
{
    private readonly BufferFactory factory = BufferFactory.Shared;

    [Fact]
    public void CreateBuffer_Creates_Buffer()
    {
        var bufferSize = 50;

        var buffer = factory.CreateBuffer(bufferSize);

        Assert.NotNull(buffer);
    }
}
