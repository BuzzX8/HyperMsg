using System;
using System.Buffers;

namespace HyperMsg.Transciever
{
    public class MessageWorker<T>
    {
        private readonly BackgroundWorker worker;
        private readonly IPipeReader reader;
        private readonly Action<T> messageHandler;

        public MessageWorker(IPipeReader reader, Func<ReadOnlySequence<byte>, DeserializationResult<T>> deserializeAction, Action<T> messageHandler)
        {
            this.reader = reader;
            worker = CreateBgWorker(deserializeAction);
            this.messageHandler = messageHandler ?? throw new ArgumentNullException(nameof(messageHandler));
        }

        public IDisposable Run() => worker.Run();

        private BackgroundWorker CreateBgWorker(Func<ReadOnlySequence<byte>, DeserializationResult<T>> deserializeAction)
        {
            var bufferReader = new BufferReader<T>(deserializeAction, OnMessageRead);
            var readPipeAction = new ReadPipeAction(reader, bufferReader.ReadBuffer);
            return new BackgroundWorker(readPipeAction.InvokeAsync);
        }

        private void OnMessageRead(T message)
        {
            messageHandler.Invoke(message);
        }
    }
}
