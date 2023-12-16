using LanguageExt;
using LanguageExt.Common;
using Xunit;

namespace HyperMsg;

public class EncodingWriterTests
{
    [Fact]
    public void New_Creates_Encoding_Writer()
    {
        var buffer = new byte[1024];
        var expected = Guid.NewGuid();

        var writer = EncodingWriter.New<Guid>(
            (buffer, message) =>
            {
                message.ToByteArray().CopyTo(buffer);
                return new Result<int>(message.ToByteArray().Length);
            },
            buffer => new Result<Unit>(Unit.Default), buffer);

        var result = writer(expected);

        Assert.True(result.IsSuccess);
        Assert.Equal(expected.ToByteArray(), buffer[..expected.ToByteArray().Length]);
    }

    [Fact]
    public async void NewAsync_Creates_Encoding_Writer()
    {
        var buffer = new byte[1024];
        var expected = Guid.NewGuid();

        var writer = EncodingWriter.NewAsync<Guid>(
            (buffer, message) =>
            {
                message.ToByteArray().CopyTo(buffer);
                return new Result<int>(message.ToByteArray().Length);
            },
            (buffer, token) => ValueTask.FromResult(new Result<Unit>(Unit.Default)), buffer);

        var result = await writer(expected, default);

        Assert.True(result.IsSuccess);
        Assert.Equal(expected.ToByteArray(), buffer[..expected.ToByteArray().Length]);
    }
}
