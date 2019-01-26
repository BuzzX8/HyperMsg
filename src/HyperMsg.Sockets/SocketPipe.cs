using System;
using System.Buffers;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Sockets
{
    public class SocketPipe
    {
        private readonly ISocket socket;

        public SocketPipe(ISocket socket)
        {            
            this.socket = socket ?? throw new ArgumentNullException(nameof(socket));
        }

        public void DoWork(object sender, DoWorkEventArgs e)
        {

        }

        public int Read(ReadOnlySequence<byte> buffer)
        {
            return -1;
        }

        public int Write(ReadOnlySequence<byte> buffer)
        {
            if (buffer.IsSingleSegment)
            {
                socket.Write(buffer.First);
                return buffer.First.Length;
            }

            var enumerator = buffer.GetEnumerator();

            while (enumerator.MoveNext())
            {
                socket.Write(enumerator.Current);
            }

            return (int)buffer.Length;
        }
    }
}
