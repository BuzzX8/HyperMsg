namespace HyperMsg.Buffers;

public class BufferingContextTests
{
    private const ulong InputBufferSize = 16;
    private const ulong OutputBufferSize = 32;

    [Fact]
    public void Input_ReturnsInputBufferInstance()
    {
        var context = new BufferingContext(InputBufferSize, OutputBufferSize);

        var input = context.Input;

        Assert.NotNull(input);
        Assert.IsAssignableFrom<IBuffer>(input);
    }

    [Fact]
    public void Output_ReturnsOutputBufferInstance()
    {
        var context = new BufferingContext(InputBufferSize, OutputBufferSize);

        var output = context.Output;

        Assert.NotNull(output);
        Assert.IsAssignableFrom<IBuffer>(output);
    }

    [Fact]
    public void InputHandlers_InitializesAsEmptyCollection()
    {
        var context = new BufferingContext(InputBufferSize, OutputBufferSize);

        Assert.NotNull(context.InputHandlers);
        Assert.Empty(context.InputHandlers);
    }

    [Fact]
    public void OutputHandlers_InitializesAsEmptyCollection()
    {
        var context = new BufferingContext(InputBufferSize, OutputBufferSize);

        Assert.NotNull(context.OutputHandlers);
        Assert.Empty(context.OutputHandlers);
    }

    [Fact]
    public async Task RequestInputBufferHandling_InvokesAllInputHandlers()
    {
        var context = new BufferingContext(InputBufferSize, OutputBufferSize);
        var buffer = context.Input;
        var callOrder = new List<int>();

        context.InputHandlers.Add(async (b, ct) => { callOrder.Add(1); await Task.Yield(); });
        context.InputHandlers.Add(async (b, ct) => { callOrder.Add(2); await Task.Yield(); });

        await context.RequestInputBufferHandling();

        Assert.Equal([ 1, 2 ], callOrder);
    }

    [Fact]
    public async Task RequestOutputBufferHandling_InvokesAllOutputHandlers()
    {
        var context = new BufferingContext(InputBufferSize, OutputBufferSize);
        var buffer = context.Output;
        var callOrder = new List<int>();

        context.OutputHandlers.Add(async (b, ct) => { callOrder.Add(1); await Task.Yield(); });
        context.OutputHandlers.Add(async (b, ct) => { callOrder.Add(2); await Task.Yield(); });

        await context.RequestOutputBufferHandling();

        Assert.Equal(new[] { 1, 2 }, callOrder);
    }
}
