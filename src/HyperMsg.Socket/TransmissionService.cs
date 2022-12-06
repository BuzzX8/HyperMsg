﻿using System.Net.Sockets;

namespace HyperMsg.Socket;

public class TransmissionService : Service
{
    private readonly SocketHolder socketHolder;
    private readonly SocketAsyncEventArgs asyncEventArgs;

    public TransmissionService(ITopic topic, SocketHolder socketHolder) : base(topic)
    {
        this.socketHolder = socketHolder;
        asyncEventArgs = new();
        asyncEventArgs.Completed += OperationCompleted;
    }

    private void OperationCompleted(object? _, SocketAsyncEventArgs eventArgs)
    {
        switch (eventArgs.LastOperation)
        {
            case SocketAsyncOperation.Send:
                Dispatch(new SendResult(eventArgs.BytesTransferred, eventArgs.SocketError));
                break;

            case SocketAsyncOperation.Receive:
                Dispatch(new ReceiveResult(eventArgs.BytesTransferred, eventArgs.SocketError));
                break;
        }
    }

    protected override void RegisterHandlers(IRegistry registry)
    {
        registry.Register<SendRequest>(Send);
        registry.Register<ReceiveRequest>(Receive);
    }

    protected override void UnregisterHandlers(IRegistry registry)
    {
        registry.Unregister<SendRequest>(Send);
        registry.Unregister<ReceiveRequest>(Receive);
    }

    private void Send(SendRequest message)
    {
        if (message.Buffer.Length is 0)
        {
            return;
        }

        asyncEventArgs.SetBuffer(message.Buffer);
        if (!socketHolder.Socket.SendAsync(asyncEventArgs))
        {
            OperationCompleted(socketHolder, asyncEventArgs);
        }
    }

    private void Receive(ReceiveRequest message)
    {
        asyncEventArgs.SetBuffer(message.Buffer);
        if (!socketHolder.Socket.ReceiveAsync(asyncEventArgs))
        {
            OperationCompleted(socketHolder, asyncEventArgs);
        }
    }

    public override void Dispose()
    {
        base.Dispose();
        asyncEventArgs.Completed -= OperationCompleted;
        asyncEventArgs.Dispose();
    }
}

public record struct SendRequest(Memory<byte> Buffer);

public record struct SendResult(int BytesTransferred, SocketError Error);

public record struct ReceiveRequest(Memory<byte> Buffer);

public record struct ReceiveResult(int BytesTransferred, SocketError Error);