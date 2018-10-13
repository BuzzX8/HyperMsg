using System.IO.Pipelines;
using System.Threading.Tasks;

namespace HyperMsg
{
    interface IMessageBuffer<T>
    {
        void Write(T message);
        Task<FlushResult> FlushAsync();
    }
}
