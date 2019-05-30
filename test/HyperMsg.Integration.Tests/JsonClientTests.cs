﻿using HyperMsg.Sockets;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Xunit;

namespace HyperMsg.Integration
{
    public class JsonClientTests : IDisposable
    {
        private readonly IJsonClient client;
        private readonly ConnectionListener listener;
        private readonly IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, 8080);
        private readonly byte[] receivingBuffer;

        private Socket acceptedSocket;        

        public JsonClientTests()
        {            
            var clientBuilder = new ConfigurableBuilder<IJsonClient>();
            clientBuilder.UseCoreServices<JObject>(2048, 2048);
            clientBuilder.UseSockets(endPoint);
            clientBuilder.UseJsonSerializer();
            clientBuilder.UseJsonClient();

            client = clientBuilder.Build();

            listener = new ConnectionListener();
            receivingBuffer = new byte[100];
        }

        [Fact]
        public void Connect_Establishes_Connection_With_Target()
        {
            StartListenerAndConnectClient();

            Assert.NotNull(acceptedSocket);
        }

        [Fact]
        public void SendObject_Serializes_And_Transmits_JObject()
        {            
            var expectedMessage = JObject.Parse("{ Subject: 'Hello', Message: 'World' }");
            StartListenerAndConnectClient();

            client.SendObject(expectedMessage);
            
            int received = acceptedSocket.Receive(receivingBuffer);
            var actualMessage = JObject.Parse(Encoding.UTF8.GetString(new ReadOnlySpan<byte>(receivingBuffer, 0, received)));

            Assert.Equal(expectedMessage, actualMessage);
        }

        [Fact]
        public void ObjectReceived_Rises_When_Received_Json_Data()
        {
            var expectedMessage = "{ Subject: 'Hello', Message: 'World' }";
            var receivedObject = default(JObject);
            var @event = new ManualResetEventSlim();
            client.ObjectReceived += (s, e) =>
            {
                receivedObject = e.Object;
                @event.Set();
            };
            StartListenerAndConnectClient();

            acceptedSocket.Send(Encoding.UTF8.GetBytes(expectedMessage));
            @event.Wait(TimeSpan.FromSeconds(2));

            Assert.NotNull(receivedObject);
            Assert.Equal(JObject.Parse(expectedMessage), receivedObject);
        }

        private void StartListenerAndConnectClient()
        {
            var @event = new ManualResetEventSlim();

            listener.ConnectionAccepted += s =>
            {
                acceptedSocket = s;
                @event.Set();
            };
            listener.StartListening(endPoint);

            client.Connect();
            @event.Wait();
        }

        public void Dispose()
        {
            client.Disconnect();
            acceptedSocket?.Close();
            listener.StopListening();
        }
    }
}