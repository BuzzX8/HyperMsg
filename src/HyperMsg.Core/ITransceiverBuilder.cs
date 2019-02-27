using System;

namespace HyperMsg
{
    public interface ITransceiverBuilder<in TSend, TReceive>
    {
        void Configure(Action<BuilderContext> configurator);

        ITransceiver<TSend, TReceive> Build();
    }
}