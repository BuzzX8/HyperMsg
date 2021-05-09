using System;

namespace HyperMsg.Messages
{
    public struct WriteToBufferCommand
    {
        public WriteToBufferCommand(Action<IWriteToBufferCommandHandler> writeToBufferAction) => WriteToBufferAction = writeToBufferAction;

        public Action<IWriteToBufferCommandHandler> WriteToBufferAction { get; }
    }
}
