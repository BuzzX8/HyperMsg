namespace HyperMsg.Buffers;

public class BufferingContextTests
{
    [Fact]
    public void Constructor_Initializes_Input_And_Output_Buffers()
    {
        var ctx = new BufferingContext();

        Assert.NotNull(ctx.Input);
        Assert.NotNull(ctx.Output);
    }

    [Fact]
    public void Default_Buffers_Have_AtLeast_Default_Capacity()
    {
        const int OneMiB = 1024 * 1024;
        var ctx = new BufferingContext();

        var inputCapacity = ctx.Input.Writer.GetMemory(1).Length;
        var outputCapacity = ctx.Output.Writer.GetMemory(1).Length;

        Assert.True(inputCapacity >= OneMiB, $"Input capacity {inputCapacity} is less than {OneMiB}");
        Assert.True(outputCapacity >= OneMiB, $"Output capacity {outputCapacity} is less than {OneMiB}");
    }

    [Fact]
    public async Task RequestInputBufferHandling_Rises_InputBufferHandlingRequested_Event()
    {
        var ctx = new BufferingContext();

        var eventRaised = false;
        ctx.InputBufferHandlingRequested += (buffer, ct) =>
        {
            eventRaised = true;
            return ValueTask.CompletedTask;
        };

        await ctx.RequestInputBufferHandling();

        Assert.True(eventRaised, "InputBufferHandlingRequested event was not raised.");
    }

    [Fact]
    public async Task RequestOutputBufferHandling_Rises_OutputBufferHandlingRequested()
    {
        var ctx = new BufferingContext();

        var eventRaised = false;
        ctx.OutputBufferHandlingRequested += (buffer, ct) =>
        {
            eventRaised = true;
            return ValueTask.CompletedTask;
        };

        await ctx.RequestOutputBufferHandling();

        Assert.True(eventRaised, "OutputBufferHandlingRequested event was not raised.");
    }

    [Fact]
    public async Task RequestInputBufferUpdate_Raises_InputBufferUpdateRequested_Event_And_Forwards_Buffer_And_CancellationToken()
    {
        var ctx = new BufferingContext();

        IBuffer? received = null;
        var observedCancellation = false;

        ctx.InputBufferUpdateRequested += (buffer, ct) =>
        {
            received = buffer;
            observedCancellation = ct.IsCancellationRequested;
            return ValueTask.CompletedTask;
        };

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await ctx.RequestInputBufferUpdate(cts.Token);

        Assert.Same(ctx.Input, received);
        Assert.True(observedCancellation, "CancellationToken was not forwarded to the input update handler.");
    }

    [Fact]
    public async Task RequestOutputBufferUpdate_Raises_OutputBufferUpdateRequested_Event_And_Forwards_Buffer_And_CancellationToken()
    {
        var ctx = new BufferingContext();

        IBuffer? received = null;
        var observedCancellation = false;

        ctx.OutputBufferUpdateRequested += (buffer, ct) =>
        {
            received = buffer;
            observedCancellation = ct.IsCancellationRequested;
            return ValueTask.CompletedTask;
        };

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await ctx.RequestOutputBufferUpdate(cts.Token);

        Assert.Same(ctx.Output, received);
        Assert.True(observedCancellation, "CancellationToken was not forwarded to the output update handler.");
    }
}
