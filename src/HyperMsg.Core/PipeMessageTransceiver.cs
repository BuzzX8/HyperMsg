using System;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class PipeMessageTransceiver<T> : IMessageSender<T>, IDisposable
    {
        private readonly IMessageBuffer<T> messageBuffer;
        private readonly RunAction run1;
        private readonly RunAction run2;

        private IDisposable disp1;
        private IDisposable disp2;

        public PipeMessageTransceiver(IMessageBuffer<T> messageBuffer, RunAction run1, RunAction run2)
        {
            this.messageBuffer = messageBuffer;
            this.run1 = run1;
            this.run2 = run2;
        }
        
        public IDisposable Run()
        {
            if (disp1 != null)
            {
                return this;
            }

            disp1 = run1();
            disp2 = run2();

            return this;
        }

        public async Task SendAsync(T message, CancellationToken token = default)
        {
            messageBuffer.Write(message);
            var flushResult = await messageBuffer.FlushAsync(token);
            OnFlushResult(flushResult);
        }

        private void OnFlushResult(FlushResult flushResult)
        { }

        public void Dispose()
        {
            if (disp1 == null)
            {
                return;
            }

            disp1.Dispose();
            disp2.Dispose();
        }
    }

    public delegate IDisposable RunAction();
}
