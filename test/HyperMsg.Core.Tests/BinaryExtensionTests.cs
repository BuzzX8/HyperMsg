using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Xunit;

using BE=HyperMsg.BinaryExtensions;

namespace HyperMsg
{
    public class BinaryExtensionTests
    {
	    public static IEnumerable<object[]> GetTestCasesForRead()
	    {
			//Read Big endian tests
		    yield return GetTestCaseForRead<short>((b, o) => b.ReadInt16BigEndian(o), 0, 0, 0, 0);
		    yield return GetTestCaseForRead<short>((b, o) => b.ReadInt16BigEndian(o), short.MinValue, 0, 0x80, 0x00);
			yield return GetTestCaseForRead<short>((b, o) => b.ReadInt16BigEndian(o), short.MaxValue, 0, 0x7f, 0xff);

		    yield return GetTestCaseForRead((b, o) => b.ReadInt32BigEndian(o), 0, 0, 0, 0, 0, 0);
		    yield return GetTestCaseForRead((b, o) => b.ReadInt32BigEndian(o), int.MinValue, 0, 0x80, 0, 0, 0);
		    yield return GetTestCaseForRead((b, o) => b.ReadInt32BigEndian(o), int.MaxValue, 0, 0x7f, 0xff, 0xff, 0xff);

		    yield return GetTestCaseForRead<long>((b, o) => b.ReadInt64BigEndian(o), 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
		    yield return GetTestCaseForRead<long>((b, o) => b.ReadInt64BigEndian(o), long.MinValue, 0, 0x80, 0, 0, 0, 0, 0, 0, 0);
		    yield return GetTestCaseForRead<long>((b, o) => b.ReadInt64BigEndian(o), long.MaxValue, 0, 0x7f, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff);

			yield return GetTestCaseForRead<ushort>((b, o) => b.ReadUInt16BigEndian(o), 0, 0, 0, 0);
		    yield return GetTestCaseForRead<ushort>((b, o) => b.ReadUInt16BigEndian(o), ushort.MaxValue - 1, 0, 0xff, 0xfe);

		    yield return GetTestCaseForRead<uint>((b, o) => b.ReadUInt32BigEndian(o), 0, 0, 0, 0, 0, 0);
		    yield return GetTestCaseForRead<uint>((b, o) => b.ReadUInt32BigEndian(o), uint.MaxValue - 1, 0, 0xff, 0xff, 0xff, 0xfe);

		    yield return GetTestCaseForRead<ulong>((b, o) => b.ReadUInt64BigEndian(o), 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
		    yield return GetTestCaseForRead<ulong>((b, o) => b.ReadUInt64BigEndian(o), ulong.MaxValue - 1, 0, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xfe);

			//Read Little endian tests
		    yield return GetTestCaseForRead<short>((b, o) => b.ReadInt16LittleEndian(o), 0, 0, 0, 0);
		    yield return GetTestCaseForRead<short>((b, o) => b.ReadInt16LittleEndian(o), short.MinValue, 0, 0x00, 0x80);
		    yield return GetTestCaseForRead<short>((b, o) => b.ReadInt16LittleEndian(o), short.MaxValue, 0, 0xff, 0x7f);

		    yield return GetTestCaseForRead((b, o) => b.ReadInt32LittleEndian(o), 0, 0, 0, 0, 0, 0);
		    yield return GetTestCaseForRead((b, o) => b.ReadInt32LittleEndian(o), int.MinValue, 0, 0, 0, 0, 0x80);
		    yield return GetTestCaseForRead((b, o) => b.ReadInt32LittleEndian(o), int.MaxValue, 0, 0xff, 0xff, 0xff, 0x7f);

		    yield return GetTestCaseForRead<long>((b, o) => b.ReadInt64LittleEndian(o), 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
		    yield return GetTestCaseForRead<long>((b, o) => b.ReadInt64LittleEndian(o), long.MinValue, 0, 0, 0, 0, 0, 0, 0, 0, 0x80);
		    yield return GetTestCaseForRead<long>((b, o) => b.ReadInt64LittleEndian(o), long.MaxValue, 0, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0x7f);

		    yield return GetTestCaseForRead<ushort>((b, o) => b.ReadUInt16LittleEndian(o), 0, 0, 0, 0);
			yield return GetTestCaseForRead<ushort>((b, o) => b.ReadUInt16LittleEndian(o), ushort.MaxValue - 1, 0, 0xfe, 0xff);

		    yield return GetTestCaseForRead<uint>((b, o) => b.ReadUInt32LittleEndian(o), 0, 0, 0, 0, 0, 0);
		    yield return GetTestCaseForRead<uint>((b, o) => b.ReadUInt32LittleEndian(o), uint.MaxValue - 1, 0, 0xfe, 0xff, 0xff, 0xff);

		    yield return GetTestCaseForRead<ulong>((b, o) => b.ReadUInt64LittleEndian(o), 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
		    yield return GetTestCaseForRead<ulong>((b, o) => b.ReadUInt64LittleEndian(o), ulong.MaxValue - 1, 0, 0xfe, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff);
		}

	    public static object[] GetTestCaseForRead<T>(Func<ReadOnlyMemory<byte>, int, object> read, T expected, int offset, params byte[] serialized)
	    {
		    return new object[] { expected, offset, serialized, read };
	    }

	    [Theory]
		[MemberData(nameof(GetTestCasesForRead))]
	    public void ReadEndian_Correctly_Deserializes_Value(object expected, int offset, byte[] serialized, Func<ReadOnlyMemory<byte>, int, object> read)
	    {
		    var actual = read(new ReadOnlyMemory<byte>(serialized), offset);

			Assert.Equal(expected, actual);
	    }

	    public static IEnumerable<object[]> GetTestCasesForWrite()
	    {
			//Big endian write
		    yield return GetTestCaseForWrite<short>(BE.WriteInt16BigEndian, 0, 0, 0);
		    yield return GetTestCaseForWrite<short>(BE.WriteInt16BigEndian, short.MinValue, 0x80, 0);
		    yield return GetTestCaseForWrite<short>(BE.WriteInt16BigEndian, short.MaxValue, 0x7f, 0xff);

		    yield return GetTestCaseForWrite(BE.WriteInt32BigEndian, 0, 0, 0, 0, 0);
		    yield return GetTestCaseForWrite(BE.WriteInt32BigEndian, int.MinValue, 0x80, 0, 0, 0);
		    yield return GetTestCaseForWrite(BE.WriteInt32BigEndian, int.MaxValue, 0x7f, 0xff, 0xff, 0xff);

		    yield return GetTestCaseForWrite<long>(BE.WriteInt64BigEndian, 0, 0, 0, 0, 0, 0, 0, 0, 0);
		    yield return GetTestCaseForWrite<long>(BE.WriteInt64BigEndian, long.MinValue, 0x80, 0, 0, 0, 0, 0, 0, 0);
		    yield return GetTestCaseForWrite<long>(BE.WriteInt64BigEndian, long.MaxValue, 0x7f, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff);
			
		    yield return GetTestCaseForWrite<ushort>(BE.WriteUInt16BigEndian, ushort.MinValue, 0, 0);
		    yield return GetTestCaseForWrite<ushort>(BE.WriteUInt16BigEndian, ushort.MaxValue - 1, 0xff, 0xfe);
			
		    yield return GetTestCaseForWrite<uint>(BE.WriteUInt32BigEndian, uint.MinValue, 0, 0, 0, 0);
		    yield return GetTestCaseForWrite<uint>(BE.WriteUInt32BigEndian, uint.MaxValue - 1, 0xff, 0xff, 0xff, 0xfe);
			
		    yield return GetTestCaseForWrite<ulong>(BE.WriteUInt64BigEndian, ulong.MinValue, 0, 0, 0, 0, 0, 0, 0, 0);
		    yield return GetTestCaseForWrite<ulong>(BE.WriteUInt64BigEndian, ulong.MaxValue - 1, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xfe);

			//Little endian write
		    yield return GetTestCaseForWrite<short>(BE.WriteInt16LittleEndian, 0, 0, 0);
		    yield return GetTestCaseForWrite<short>(BE.WriteInt16LittleEndian, short.MinValue, 0, 0x80);
		    yield return GetTestCaseForWrite<short>(BE.WriteInt16LittleEndian, short.MaxValue, 0xff, 0x7f);

		    yield return GetTestCaseForWrite(BE.WriteInt32LittleEndian, 0, 0, 0, 0, 0);
		    yield return GetTestCaseForWrite(BE.WriteInt32LittleEndian, int.MinValue, 0, 0, 0, 0x80);
		    yield return GetTestCaseForWrite(BE.WriteInt32LittleEndian, int.MaxValue, 0xff, 0xff, 0xff, 0x7f);

		    yield return GetTestCaseForWrite<long>(BE.WriteInt64LittleEndian, 0, 0, 0, 0, 0, 0, 0, 0, 0);
		    yield return GetTestCaseForWrite<long>(BE.WriteInt64LittleEndian, long.MinValue, 0, 0, 0, 0, 0, 0, 0, 0x80);
		    yield return GetTestCaseForWrite<long>(BE.WriteInt64LittleEndian, long.MaxValue, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0x7f);
			
		    yield return GetTestCaseForWrite<ushort>(BE.WriteUInt16LittleEndian, ushort.MinValue, 0, 0);
		    yield return GetTestCaseForWrite<ushort>(BE.WriteUInt16LittleEndian, ushort.MaxValue - 1, 0xfe, 0xff);
			
		    yield return GetTestCaseForWrite<uint>(BE.WriteUInt32LittleEndian, uint.MinValue, 0, 0, 0, 0);
		    yield return GetTestCaseForWrite<uint>(BE.WriteUInt32LittleEndian, uint.MaxValue - 1, 0xfe, 0xff, 0xff, 0xff);
			
		    yield return GetTestCaseForWrite<ulong>(BE.WriteUInt64LittleEndian, ulong.MinValue, 0, 0, 0, 0, 0, 0, 0, 0);
		    yield return GetTestCaseForWrite<ulong>(BE.WriteUInt64LittleEndian, ulong.MaxValue - 1, 0xfe, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff);
		}

	    public static object[] GetTestCaseForWrite<T>(Action<Memory<byte>, T, int> write, T value, params byte[] expected)
	    {
		    return new object[] {value, expected, (Action<Memory<byte>, object, int>)((b, v, o) => write(b, (T)v, o))};
	    }

	    [Theory]
		[MemberData(nameof(GetTestCasesForWrite))]
	    public void WriteEndian_Correctly_Serializes_Value(object value, byte[] expected, Action<Memory<byte>, object, int> write)
	    {
			var buffer = new Memory<byte>(new byte[expected.Length]);

		    write(buffer, value, 0);

			Assert.Equal(expected, buffer.ToArray());
	    }
    }
}
