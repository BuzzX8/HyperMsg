using System;

namespace HyperMsg
{
    public interface ITransceiverBuilder<in TSend, out TReceive>
    {
        void Configure(Action<BuilderContext> configurator);

        ITransceiver<TSend, TReceive> Build();
    }
}