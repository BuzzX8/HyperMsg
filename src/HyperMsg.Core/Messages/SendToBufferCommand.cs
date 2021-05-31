using System;

namespace HyperMsg.Messages
{
    public struct SendToBufferCommand
    {
        public SendToBufferCommand(Action<BufferService> writeToBufferAction) => WriteToBufferAction = writeToBufferAction;

        public Action<BufferService> WriteToBufferAction { get; }
    }
}
