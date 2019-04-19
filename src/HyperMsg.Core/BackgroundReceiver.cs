using System;

namespace HyperMsg
{
    public class BackgroundReceiver<T> : IDisposable
    {
        private readonly IReceiver<T> messageReceiver;
        private Action<T> messageHandler;

        public BackgroundReceiver(IReceiver<T> messageReceiver)
        {
            this.messageReceiver = messageReceiver ?? throw new ArgumentNullException(nameof(messageReceiver));
        }

        public IDisposable Run(Action<T> messageHandler)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        { }

        public Action<Exception> OnUnhandlerException;
    }
}
