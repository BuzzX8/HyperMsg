using System.Buffers;

namespace HyperMsg
{
    public static class ConfigurableExtensions
    {
        public static void UseCoreServices<T>(this IConfigurable configurable, int receivingBufferSize, int sendingBufferSize)
        {
            configurable.UseSharedMemoryPool();
            configurable.UseBuffers(receivingBufferSize, sendingBufferSize);
        }

        public static void UseSharedMemoryPool(this IConfigurable configurable) => configurable.RegisterService(typeof(MemoryPool<byte>), (p, s) => MemoryPool<byte>.Shared);

        public static void UseBuffers(this IConfigurable configurable, int receivingBufferSize, int sendingBufferSize)
        {
            const string ReceivingBufferSetting = "ReceivingBufferSize";
            const string SendingBufferSetting = "SendingBufferSize";

            configurable.AddSetting(ReceivingBufferSetting, receivingBufferSize);
            configurable.AddSetting(SendingBufferSetting, sendingBufferSize);
            configurable.RegisterService(typeof(IReceivingBuffer), (p, s) =>
            {
                var bufferSize = (int)s[ReceivingBufferSetting];
                var memoryPool = (MemoryPool<byte>)p.GetService(typeof(MemoryPool<byte>));

                return new Buffer(memoryPool.Rent(bufferSize));
            });
            configurable.RegisterService(typeof(ISendingBuffer), (p, s) =>
            {
                var bufferSize = (int)s[SendingBufferSetting];
                var memoryPool = (MemoryPool<byte>)p.GetService(typeof(MemoryPool<byte>));

                return new Buffer(memoryPool.Rent(bufferSize));
            });
        }
    }
}
