using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace HyperMsg.Integration
{
    public class JsonClient : IJsonClient, IHandler<JObject>
    {
        private readonly IMessageBuffer<JObject> messageBuffer;
        private readonly IHandler<TransportCommands> transportHandler;
        private readonly IHandler<ReceiveMode> receiveModeHandler;

        public JsonClient(IMessageBuffer<JObject> messageBuffer, IHandler<TransportCommands> transportHandler, IHandler<ReceiveMode> receiveModeHandler)
        {
            this.messageBuffer = messageBuffer;
            this.transportHandler = transportHandler;
            this.receiveModeHandler = receiveModeHandler;
        }

        public void Connect()
        {
            transportHandler.Handle(TransportCommands.OpenConnection);
            receiveModeHandler.Handle(ReceiveMode.Reactive);
        }

        public async Task ConnectAsync(CancellationToken cancellationToken)
        {
            await transportHandler.HandleAsync(TransportCommands.OpenConnection, cancellationToken);
            await receiveModeHandler.HandleAsync(ReceiveMode.Reactive, cancellationToken);
        }

        public void Disconnect() => transportHandler.Handle(TransportCommands.CloseConnection);

        public Task DisconnectAsync(CancellationToken cancellationToken) => transportHandler.HandleAsync(TransportCommands.CloseConnection, cancellationToken);

        public void SendObject(JObject @object)
        {
            messageBuffer.Write(@object);
            messageBuffer.Flush();
        }

        public Task SendObjectAsync(JObject @object, CancellationToken cancellationToken)
        {
            messageBuffer.Write(@object);
            return messageBuffer.FlushAsync(cancellationToken);
        }

        public void SendObjects(IEnumerable<JObject> objects)
        {
            foreach (var @object in objects)
            {
                messageBuffer.Write(@object);
            }

            messageBuffer.Flush();
        }

        public Task SendObjectsAsync(IEnumerable<JObject> objects, CancellationToken cancellationToken)
        {
            foreach (var @object in objects)
            {
                messageBuffer.Write(@object);
            }

            return messageBuffer.FlushAsync(cancellationToken);
        }

        public void Handle(JObject message)
        {
            throw new NotImplementedException();
        }

        public Task HandleAsync(JObject message, CancellationToken token = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public event EventHandler<ObjectReceivedEventArgs> ObjectReceived;
    }
}
