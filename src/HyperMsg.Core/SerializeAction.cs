using System.Buffers;

namespace HyperMsg
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="writer"></param>
    /// <param name="message"></param>
    public delegate void SerializeAction<T>(IBufferWriter<byte> writer, T message);
}