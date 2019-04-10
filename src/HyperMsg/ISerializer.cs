using System.Buffers;

namespace HyperMsg
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISerializer<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        DeserializationResult<T> Deserialize(ReadOnlySequence<byte> buffer);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="message"></param>
        void Serialize(IBufferWriter<byte> writer, T message);
    }
}