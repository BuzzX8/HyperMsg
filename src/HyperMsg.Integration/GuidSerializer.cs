using System;
using System.Buffers;

namespace HyperMsg.Integration
{
    public class GuidSerializer : ISerializer<Guid>
    {
        public DeserializationResult<Guid> Deserialize(ReadOnlySequence<byte> buffer)
        {
            const int GuidSize = 16;

            if (buffer.Length < GuidSize)
            {
                return new DeserializationResult<Guid>(0, Guid.Empty);
            }

            var slice = buffer.First.Slice(0, GuidSize);

            return new DeserializationResult<Guid>(GuidSize, new Guid(slice.ToArray()));
        }

        public void Serialize(IBufferWriter<byte> writer, Guid message)
        {
            var bytes = message.ToByteArray();
            var buffer = writer.GetSpan(bytes.Length);
            bytes.CopyTo(buffer);
            writer.Advance(bytes.Length);
        }
    }
}
