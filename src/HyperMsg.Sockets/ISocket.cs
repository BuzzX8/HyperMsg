﻿using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Sockets
{
    public interface ISocket
    {
        bool IsConnected { get; }

        Stream Stream { get; }

        void Connect();

        Task ConnectAsync(CancellationToken token);

        void Disconnect();

        Task DisconnectAsync(CancellationToken token);
    }
}