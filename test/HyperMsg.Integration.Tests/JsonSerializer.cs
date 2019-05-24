using Newtonsoft.Json.Linq;
using System;
using System.Buffers;
using System.Text;

namespace HyperMsg.Integration
{
    public class JsonSerializer : ISerializer<JObject>
    {
        public DeserializationResult<JObject> Deserialize(ReadOnlySequence<byte> buffer)
        {
            throw new NotImplementedException();
        }

        public void Serialize(IBufferWriter<byte> writer, JObject message)
        {
            var bytes = Encoding.UTF8.GetBytes(message.ToString());
            var buffer = writer.GetSpan(bytes.Length);
            bytes.CopyTo(buffer);
            writer.Advance(bytes.Length);
        }
    }
}
