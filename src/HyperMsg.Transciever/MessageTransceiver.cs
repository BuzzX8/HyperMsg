using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Transciever
{
    public class MessageTransceiver<T> : ITransceiver<T, T>, IDisposable
    {
        private readonly IMessageBuffer<T> messageBuffer;
        private readonly ISubject<T> messageSubject;
        private List<Func<IDisposable>> runners;
        private List<IDisposable> handlers;

        private readonly Lazy<ReadBufferAction> readBuffer;

        public MessageTransceiver(IMessageBuffer<T> messageBuffer, MessageReceiverFactory<T> messageReceiverFactory, ISubject<T> messageSubject, ICollection<Func<IDisposable>> runners)
        {
            this.messageBuffer = messageBuffer ?? throw new ArgumentNullException(nameof(messageBuffer));
            this.messageSubject = messageSubject ?? throw new ArgumentNullException(nameof(messageSubject));
            this.runners = new List<Func<IDisposable>>();
            this.runners.AddRange(runners);
            handlers = new List<IDisposable>();
            readBuffer = new Lazy<ReadBufferAction>(() => messageReceiverFactory.Invoke(OnMessageReceived));
        }        

        public IDisposable Run()
        {
            RunChildRunnersIfRequired();
            return this;
        }

        public void Send(T message) => SendAsync(message).GetAwaiter().GetResult();

        public async Task SendAsync(T message, CancellationToken token = default)
        {
            messageBuffer.Write(message);
            await messageBuffer.FlushAsync(token);
        }

        public int ReadBuffer(ReadOnlySequence<byte> buffer) => readBuffer.Value.Invoke(buffer);

        private void RunChildRunnersIfRequired()
        {
            if (handlers.Any())
            {
                return;
            }

            foreach (var runner in runners)
            {
                var handle = runner.Invoke();
                handlers.Add(handle);
            }
        }

        public IDisposable Subscribe(IObserver<T> observer) => messageSubject.Subscribe(observer);

        public void Dispose()
        {
            foreach (var handler in handlers)
            {
                handler.Dispose();
            }
        }

        private void OnMessageReceived(T message) => messageSubject.OnNext(message);
    }

    public delegate int ReadBufferAction(ReadOnlySequence<byte> buffer);

    public delegate ReadBufferAction MessageReceiverFactory<T>(Action<T> onMessage);
}
