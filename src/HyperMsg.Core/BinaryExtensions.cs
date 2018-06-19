using System;

namespace HyperMsg
{
    public static class BinaryExtensions
    {
        public static short ReadInt16BigEndian(this ReadOnlyMemory<byte> buffer, int offset)
        {
	        return (short) ReadBigEndian(buffer, offset, sizeof(short));
        }

	    public static int ReadInt32BigEndian(this ReadOnlyMemory<byte> buffer, int offset)
	    {
		    return (int) ReadBigEndian(buffer, 0, sizeof(int));
	    }

	    public static long ReadInt64BigEndian(this ReadOnlyMemory<byte> buffer, int offset)
	    {
		    return ReadBigEndian(buffer, offset, sizeof(long));
	    }

	    public static ushort ReadUInt16BigEndian(this ReadOnlyMemory<byte> buffer, int offset)
	    {
		    return (ushort) ReadBigEndian(buffer, offset, sizeof(ushort));
	    }

	    public static uint ReadUInt32BigEndian(this ReadOnlyMemory<byte> buffer, int offset)
	    {
		    return (uint) ReadBigEndian(buffer, offset, sizeof(uint));
	    }

	    public static ulong ReadUInt64BigEndian(this ReadOnlyMemory<byte> buffer, int offset)
	    {
		    return (ulong) ReadBigEndian(buffer, offset, sizeof(ulong));
	    }

	    public static short ReadInt16LittleEndian(this ReadOnlyMemory<byte> buffer, int offset)
	    {
		    return (short) ReadLittleEndian(buffer, offset, sizeof(short));
	    }

	    public static int ReadInt32LittleEndian(this ReadOnlyMemory<byte> buffer, int offset)
	    {
		    return (int) ReadLittleEndian(buffer, offset, sizeof(int));
	    }

	    public static long ReadInt64LittleEndian(this ReadOnlyMemory<byte> buffer, int offset)
	    {
		    return ReadLittleEndian(buffer, offset, sizeof(long));
	    }

	    public static ushort ReadUInt16LittleEndian(this ReadOnlyMemory<byte> buffer, int offset)
	    {
		    return (ushort) ReadLittleEndian(buffer, offset, sizeof(ushort));
	    }

	    public static uint ReadUInt32LittleEndian(this ReadOnlyMemory<byte> buffer, int offset)
	    {
		    return (uint) ReadLittleEndian(buffer, offset, sizeof(uint));
	    }

	    public static ulong ReadUInt64LittleEndian(this ReadOnlyMemory<byte> buffer, int offset)
	    {
		    return (ulong) ReadLittleEndian(buffer, offset, sizeof(ulong));
	    }

	    private static long ReadBigEndian(ReadOnlyMemory<byte> buffer, int offset, int dataLength)
	    {
		    long result = 0;
		    var span = buffer.Span;

		    for (int i = 0; i < dataLength; i++)
		    {
			    result <<= 8;
			    result |= span[offset + i];
		    }

		    return result;
	    }

	    private static long ReadLittleEndian(ReadOnlyMemory<byte> buffer, int offset, int dataLength)
	    {
		    long result = 0;
		    var span = buffer.Span;

		    for (int i = dataLength - 1; i >= 0; i--)
		    {
			    result <<= 8;
			    result |= span[offset + i];
		    }

		    return result;
	    }
    }
}