using System;
using System.Threading.Tasks;

namespace HyperMsg.Sockets
{
    public static class SocketExtensions
    {
        public static int Read(this ISocket socket, Memory<byte> buffer)
        {
            throw new Exception();
        }

        public static Task<int> ReadAsync(this ISocket socket, Memory<byte> buffer)
        {
            throw new Exception();
        }

        public static void Write(this ISocket socket, ReadOnlyMemory<byte> buffer)
        {
            throw new Exception();
        }

        public static Task WriteAsync(this ISocket socket, ReadOnlyMemory<byte> buffer)
        {
            throw new Exception();
        }
    }
}
