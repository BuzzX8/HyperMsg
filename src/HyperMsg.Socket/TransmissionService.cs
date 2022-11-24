using System.Net.Sockets;

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
                throw new NotImplementedException();
                break;
        }
    }

    protected override void RegisterHandlers(IRegistry registry)
    {
        registry.Register<Send>(Send);
    }

    protected override void UnregisterHandlers(IRegistry registry)
    {
        registry.Unregister<Send>(Send);
    }

    private void Send(Send message)
    {
        asyncEventArgs.SetBuffer(message.Buffer);
        if (!socketHolder.Socket.SendAsync(asyncEventArgs))
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

public record struct Send(Memory<byte> Buffer);

public record struct SendResult(int BytesTransferred, SocketError Error);

public record struct Receive(Memory<byte> Buffer);
