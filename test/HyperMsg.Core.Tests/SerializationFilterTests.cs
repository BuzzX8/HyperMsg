﻿using System.Buffers;
using Xunit;

namespace HyperMsg;

public class SerializationFilterTests : HostFixture
{
    private readonly ISerializationFilter bufferFilter;

    public SerializationFilterTests() => bufferFilter = GetRequiredService<ISerializationFilter>();

    [Fact]
    public void Send_Writes_Message_To_Buffer()
    {
        var message = Guid.NewGuid();
        var bytes = default(byte[]);

        bufferFilter.AddSerializer<Guid>((writer, guid) => writer.Write(guid.ToByteArray()));
        HandlersRegistry.RegisterTransmitBufferHandler(reader => bytes = reader.Read().ToArray());

        Sender.Send(message);

        Assert.Equal(message.ToByteArray(), bytes);
    }

    [Fact]
    public async Task SendAsync_Writes_Message_To_Buffer()
    {
        var message = Guid.NewGuid();
        var bytes = default(byte[]);

        bufferFilter.AddSerializer<Guid>((writer, guid) => writer.Write(guid.ToByteArray()));
        HandlersRegistry.RegisterTransmitBufferHandler((reader, _) =>
        {
            bytes = reader.Read().ToArray();
            return Task.CompletedTask;
        });

        await Sender.SendAsync(message, default);

        Assert.Equal(message.ToByteArray(), bytes);
    }
}
