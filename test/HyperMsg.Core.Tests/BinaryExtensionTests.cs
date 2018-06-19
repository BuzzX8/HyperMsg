using System;
using System.Collections.Generic;
using Xunit;

namespace HyperMsg
{
    public class BinaryExtensionTests
    {
	    public static IEnumerable<object[]> GetTestDataForReadEndian()
	    {
			//Read Big endian tests
		    yield return GetTestCase<short>((b, o) => b.ReadInt16BigEndian(o), 0, 0, 0, 0);
		    yield return GetTestCase<short>((b, o) => b.ReadInt16BigEndian(o), short.MinValue, 0, 0x80, 0x00);
			yield return GetTestCase<short>((b, o) => b.ReadInt16BigEndian(o), short.MaxValue, 0, 0x7f, 0xff);

		    yield return GetTestCase((b, o) => b.ReadInt32BigEndian(o), 0, 0, 0, 0, 0, 0);
		    yield return GetTestCase((b, o) => b.ReadInt32BigEndian(o), int.MinValue, 0, 0x80, 0, 0, 0);
		    yield return GetTestCase((b, o) => b.ReadInt32BigEndian(o), int.MaxValue, 0, 0x7f, 0xff, 0xff, 0xff);

		    yield return GetTestCase<long>((b, o) => b.ReadInt64BigEndian(o), 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
		    yield return GetTestCase<long>((b, o) => b.ReadInt64BigEndian(o), long.MinValue, 0, 0x80, 0, 0, 0, 0, 0, 0, 0);
		    yield return GetTestCase<long>((b, o) => b.ReadInt64BigEndian(o), long.MaxValue, 0, 0x7f, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff);

			yield return GetTestCase<ushort>((b, o) => b.ReadUInt16BigEndian(o), 0, 0, 0, 0);
		    yield return GetTestCase<ushort>((b, o) => b.ReadUInt16BigEndian(o), ushort.MaxValue - 1, 0, 0xff, 0xfe);

		    yield return GetTestCase<uint>((b, o) => b.ReadUInt32BigEndian(o), 0, 0, 0, 0, 0, 0);
		    yield return GetTestCase<uint>((b, o) => b.ReadUInt32BigEndian(o), uint.MaxValue - 1, 0, 0xff, 0xff, 0xff, 0xfe);

		    yield return GetTestCase<ulong>((b, o) => b.ReadUInt64BigEndian(o), 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
		    yield return GetTestCase<ulong>((b, o) => b.ReadUInt64BigEndian(o), ulong.MaxValue - 1, 0, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xfe);

			//Read Little endian tests
		    yield return GetTestCase<short>((b, o) => b.ReadInt16LittleEndian(o), 0, 0, 0, 0);
		    yield return GetTestCase<short>((b, o) => b.ReadInt16LittleEndian(o), short.MinValue, 0, 0x00, 0x80);
		    yield return GetTestCase<short>((b, o) => b.ReadInt16LittleEndian(o), short.MaxValue, 0, 0xff, 0x7f);

		    yield return GetTestCase((b, o) => b.ReadInt32LittleEndian(o), 0, 0, 0, 0, 0, 0);
		    yield return GetTestCase((b, o) => b.ReadInt32LittleEndian(o), int.MinValue, 0, 0, 0, 0, 0x80);
		    yield return GetTestCase((b, o) => b.ReadInt32LittleEndian(o), int.MaxValue, 0, 0xff, 0xff, 0xff, 0x7f);

		    yield return GetTestCase<long>((b, o) => b.ReadInt64LittleEndian(o), 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
		    yield return GetTestCase<long>((b, o) => b.ReadInt64LittleEndian(o), long.MinValue, 0, 0, 0, 0, 0, 0, 0, 0, 0x80);
		    yield return GetTestCase<long>((b, o) => b.ReadInt64LittleEndian(o), long.MaxValue, 0, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0x7f);

		    yield return GetTestCase<ushort>((b, o) => b.ReadUInt16LittleEndian(o), 0, 0, 0, 0);
			yield return GetTestCase<ushort>((b, o) => b.ReadUInt16LittleEndian(o), ushort.MaxValue - 1, 0, 0xfe, 0xff);

		    yield return GetTestCase<uint>((b, o) => b.ReadUInt32LittleEndian(o), 0, 0, 0, 0, 0, 0);
		    yield return GetTestCase<uint>((b, o) => b.ReadUInt32LittleEndian(o), uint.MaxValue - 1, 0, 0xfe, 0xff, 0xff, 0xff);

		    yield return GetTestCase<ulong>((b, o) => b.ReadUInt64LittleEndian(o), 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
		    yield return GetTestCase<ulong>((b, o) => b.ReadUInt64LittleEndian(o), ulong.MaxValue - 1, 0, 0xfe, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff);
		}

	    public static object[] GetTestCase<T>(Func<ReadOnlyMemory<byte>, int, object> read, T expected, int offset, params byte[] serialized)
	    {
		    return new object[] { expected, offset, serialized, read };
	    }

	    [Theory]
		[MemberData(nameof(GetTestDataForReadEndian))]
	    public void ReadEndian_Correctly_Deserializes_Value(object expected, int offset, byte[] serialized, Func<ReadOnlyMemory<byte>, int, object> read)
	    {
		    var actual = read(new ReadOnlyMemory<byte>(serialized), offset);

			Assert.Equal(expected, actual);
	    }
    }
}
