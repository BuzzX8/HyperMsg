﻿using System.Net;

namespace HyperMsg.Sockets
{
    public static class ConfigurableExtensions
    {
        public static void UseSockets(this IConfigurable configurable, EndPoint endpoint)
        {
            configurable.AddSetting(nameof(EndPoint), endpoint);
            configurable.Configure(AddSocketServices);
        }

        private static void AddSocketServices(Configuration configuration)
        {
            var socket = new SocketProxy(SocketFactory.CreateTcpSocket(), (EndPoint)configuration.Settings[nameof(EndPoint)]);
            var transport = new SocketTransport(socket);
            configuration.Services.Add(ServiceDescriptor.Describe(typeof(IStream), transport));
            configuration.Services.Add(ServiceDescriptor.Describe(typeof(IHandler<TransportCommands>), transport));
        }
    }
}
