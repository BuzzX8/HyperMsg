using System;

namespace HyperMsg
{
    public interface ITransceiverBuilder<TSend, TReceive>
    {
        void Configure(Action<BuilderContext> configurator);

        ITransceiver<TSend, TReceive> Build();
    }
}