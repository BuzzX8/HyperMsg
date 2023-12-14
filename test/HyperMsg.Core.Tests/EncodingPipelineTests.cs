using LanguageExt;
using LanguageExt.Common;
using Xunit;

namespace HyperMsg;

public class EncodingPipelineTests
{
    [Fact]
    public void New_Creates_Encoding_Pipeline()
    {
        var buffer = new byte[1024];
        var expected = Guid.NewGuid();

        var encodingPipeline = EncodingPipeline.New<Guid>(
            (buffer, message) =>
            {
                message.ToByteArray().CopyTo(buffer);
                return new Result<int>(message.ToByteArray().Length);
            },
            buffer => new Result<Unit>(Unit.Default), buffer);

        var result = encodingPipeline(expected);

        Assert.True(result.IsSuccess);
        Assert.Equal(expected.ToByteArray(), buffer[..expected.ToByteArray().Length]);
    }

    [Fact]
    public async void NewAsync_Creates_Encoding_Pipeline()
    {
        var buffer = new byte[1024];
        var expected = Guid.NewGuid();

        var encodingPipeline = EncodingPipeline.NewAsync<Guid>(
            (buffer, message) =>
            {
                message.ToByteArray().CopyTo(buffer);
                return new Result<int>(message.ToByteArray().Length);
            },
            (buffer, token) => ValueTask.FromResult(new Result<Unit>(Unit.Default)), buffer);

        var result = await encodingPipeline(expected, default);

        Assert.True(result.IsSuccess);
        Assert.Equal(expected.ToByteArray(), buffer[..expected.ToByteArray().Length]);
    }
}
