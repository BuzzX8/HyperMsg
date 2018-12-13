using System;

namespace HyperMsg
{
    public interface IMessageTransceiverBuilder<T>
    {
        void Configure(Action<BuilderContext> configurator);

        IMessageTransceiver<T> Build();
    }
}
