using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Transciever
{
    public class MessageTransceiver<T> : ITransceiver<T, T>, IDisposable
    {
        private readonly IMessageBuffer<T> messageBuffer;
        private readonly IObservable<T> messageObserveble;
        private List<Func<IDisposable>> runners;
        private List<IDisposable> handlers;

        public MessageTransceiver(IMessageBuffer<T> messageBuffer, IObservable<T> messageObserveble, params Func<IDisposable>[] runners)
        {
            this.messageBuffer = messageBuffer ?? throw new ArgumentNullException(nameof(messageBuffer));
            this.messageObserveble = messageObserveble ?? throw new ArgumentNullException(nameof(messageObserveble));
            this.runners = new List<Func<IDisposable>>();
            this.runners.AddRange(runners);
            handlers = new List<IDisposable>();
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

        public IDisposable Subscribe(IObserver<T> observer) => messageObserveble.Subscribe(observer);

        public void Dispose()
        {
            foreach (var handler in handlers)
            {
                handler.Dispose();
            }
        }
    }
}
