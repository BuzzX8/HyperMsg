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
            var bytes = buffer.ToArray();
            var @object = Encoding.UTF8.GetString(bytes);
            return new DeserializationResult<JObject>(bytes.Length, JObject.Parse(@object));
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
