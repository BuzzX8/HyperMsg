using System.Buffers;

namespace HyperMsg
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    public delegate long ReadBufferAction(ReadOnlySequence<byte> buffer);
}
