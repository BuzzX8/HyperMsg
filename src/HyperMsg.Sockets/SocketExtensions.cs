using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Sockets
{
    public static class SocketExtensions
    {
        public static int Read(this ISocket socket, Memory<byte> buffer)
        {
            if (MemoryMarshal.TryGetArray<byte>(buffer, out var segment))
            {
                return socket.Stream.Read(segment.Array, segment.Offset, segment.Count);
            }

            throw new NotSupportedException();
        }

        public static Task<int> ReadAsync(this ISocket socket, Memory<byte> buffer, CancellationToken token = default)
        {
            if (MemoryMarshal.TryGetArray<byte>(buffer, out var segment))
            {
                return socket.Stream.ReadAsync(segment.Array, segment.Offset, segment.Count);
            }

            throw new NotSupportedException();
        }

        public static void Write(this ISocket socket, ReadOnlyMemory<byte> buffer)
        {
            if (MemoryMarshal.TryGetArray(buffer, out var segment))
            {
                socket.Stream.Write(segment.Array, segment.Offset, segment.Count);
            }

            throw new NotSupportedException();
        }

        public static Task WriteAsync(this ISocket socket, ReadOnlyMemory<byte> buffer, CancellationToken token = default)
        {
            if (MemoryMarshal.TryGetArray(buffer, out var segment))
            {
                return socket.Stream.WriteAsync(segment.Array, segment.Offset, segment.Count);
            }

            throw new NotSupportedException();
        }
    }
}
