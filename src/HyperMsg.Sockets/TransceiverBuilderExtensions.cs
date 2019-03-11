using System;
using System.ComponentModel;
using System.Net;

namespace HyperMsg.Sockets
{
    public static class TransceiverBuilderExtensions
    {
        public static void UseSockets<TSend, TReceive>(this ITransceiverBuilder<TSend, TReceive> transceiverBuilder, EndPoint endPoint)
        {
            transceiverBuilder.Configure(context =>
            {
                var socket = SocketFactory.CreateTcpSocket();
                var worker = new BackgroundWorker();
                //worker.DoWork += socketPipe.DoWork;

                context.Runners.Add(new BgWorkerHandle(worker).Run);
            });
        }
    }

    internal class BgWorkerHandle : IDisposable
    {
        private readonly BackgroundWorker worker;

        public BgWorkerHandle(BackgroundWorker worker)
        {
            this.worker = worker;
        }

        public IDisposable Run() => this;

        public void Dispose() => worker.CancelAsync();
    }
}
