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
        ctx.InputBufferHandlingRequested += (sender, buffer) =>
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
        ctx.OutputBufferHandlingRequested += (sender, buffer) =>
        {
            eventRaised = true;
            return ValueTask.CompletedTask;
        };

        await ctx.RequestOutputBufferHandling();

        Assert.True(eventRaised, "OutputBufferHandlingRequested event was not raised.");
    }
}
