using System;
using System.Buffers;

namespace HyperMsg.Integration
{
    public class GuidSerializer
    {
        public Guid Deserialize(ReadOnlySequence<byte> buffer)
        {
            const int GuidSize = 16;

            if (buffer.Length < GuidSize)
            {
                throw new InvalidOperationException();
            }

            var slice = buffer.First.Slice(0, GuidSize);

            return new Guid(slice.ToArray());
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
