using Newtonsoft.Json.Linq;
using System;
using System.Buffers;

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
            throw new NotImplementedException();
        }
    }
}
