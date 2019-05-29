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
        private readonly IHandler handler;

        public JsonClient(IMessageBuffer<JObject> messageBuffer, IHandler handler)
        {
            this.messageBuffer = messageBuffer;
            this.handler = handler;
        }

        public void Connect()
        {
            handler.Handle(TransportOperations.OpenConnection);
            handler.Handle(ReceiveMode.Reactive);
        }

        public async Task ConnectAsync(CancellationToken cancellationToken)
        {
            await handler.HandleAsync(TransportOperations.OpenConnection, cancellationToken);
            await handler.HandleAsync(ReceiveMode.Proactive, cancellationToken);
        }

        public void Disconnect() => handler.Handle(TransportOperations.CloseConnection);

        public Task DisconnectAsync(CancellationToken cancellationToken) => handler.HandleAsync(TransportOperations.CloseConnection, cancellationToken);

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
            OnObjectReceived(message);
        }

        public Task HandleAsync(JObject message, CancellationToken cancellationToken = default)
        {
            OnObjectReceived(message);
            return Task.CompletedTask;
        }

        private void OnObjectReceived(JObject @object)
        {
            ObjectReceived?.Invoke(this, new ObjectReceivedEventArgs(@object));
        }

        public event EventHandler<ObjectReceivedEventArgs> ObjectReceived;
    }
}
