using System;
using System.Collections.Generic;

namespace HyperMsg.Transciever
{
    public class TranscieverBuilder<TSend, TReceive> : ITransceiverBuilder<TSend, TReceive>
    {
        private readonly List<Action<BuilderContext>> configurators;

        public void Configure(Action<BuilderContext> configurator)
        {
            throw new NotImplementedException();
        }

        public ITransceiver<TSend, TReceive> Build()
        {
            throw new NotImplementedException();
        }
    }
}
