using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Transciever
{
    public class MessageTransceiver<T> : ITransceiver<T, T>
    {
        private readonly IPipe pipe;
        private readonly ISerializer<T> serializer;
        private readonly BackgroundWorker worker;

        public MessageTransceiver(ISerializer<T> serializer, IPipe pipe)
        {
            this.serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            this.pipe = pipe ?? throw new ArgumentNullException(nameof(pipe));

            var mListener = new MessageListener<T>(serializer.Deserialize, ReceiveMessage);
            var pipeWorkItem = new PipeReaderWorkItem(pipe.Reader, mListener.ReadBuffer);
            worker = new BackgroundWorker(pipeWorkItem.ReadPipeAsync);
        }

        public IDisposable Run() => worker.Run();

        public void Send(T message)
        {
            //serializer.Serialize(pipe.Writer, message);
            pipe.Writer.Flush();
        }

        public async Task SendAsync(T message, CancellationToken token = default)
        {
            //serializer.Serialize(pipe.Writer, message);
            await pipe.Writer.FlushAsync(token);
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            throw new NotImplementedException();
        }

        private void ReceiveMessage(T message)
        {
            throw new NotImplementedException();
        }
    }
}
