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
            handler.Publish(TransportMessage.Open);
            handler.Publish(ReceiveMode.SetReactive);
        }

        public async Task ConnectAsync(CancellationToken cancellationToken)
        {
            await handler.PublishAsync(TransportMessage.Open, cancellationToken);
            await handler.PublishAsync(ReceiveMode.SetReactive, cancellationToken);
        }

        public void Disconnect()
        {
            handler.Publish(ReceiveMode.SetProactive);
            handler.Publish(TransportMessage.Close);
        }

        public async Task DisconnectAsync(CancellationToken cancellationToken)
        {
            await handler.PublishAsync(ReceiveMode.SetProactive, cancellationToken);
            await handler.PublishAsync(TransportMessage.Close, cancellationToken);
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
