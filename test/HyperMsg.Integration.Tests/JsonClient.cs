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
        private readonly IPublisher handler;

        public JsonClient(IMessageBuffer<JObject> messageBuffer, IPublisher handler)
        {
            this.messageBuffer = messageBuffer;
            this.handler = handler;
        }

        public void Connect()
        {
            handler.Publish(TransportOperations.OpenConnection);
            handler.Publish(ReceiveMode.Reactive);
        }

        public async Task ConnectAsync(CancellationToken cancellationToken)
        {
            await handler.PublishAsync(TransportOperations.OpenConnection, cancellationToken);
            await handler.PublishAsync(ReceiveMode.Reactive, cancellationToken);
        }

        public void Disconnect()
        {
            handler.Publish(ReceiveMode.Proactive);
            handler.Publish(TransportOperations.CloseConnection);
        }

        public async Task DisconnectAsync(CancellationToken cancellationToken)
        {
            await handler.PublishAsync(ReceiveMode.Proactive, cancellationToken);
            await handler.PublishAsync(TransportOperations.CloseConnection, cancellationToken);
        }

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
