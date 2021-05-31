using System;

namespace HyperMsg.Messages
{
    public struct SendToBufferCommand
    {
        public SendToBufferCommand(Action<IWriteToBufferCommandHandler> writeToBufferAction) => WriteToBufferAction = writeToBufferAction;

        public Action<IWriteToBufferCommandHandler> WriteToBufferAction { get; }
    }
}
