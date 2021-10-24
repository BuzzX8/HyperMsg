using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    internal class SerializationFilter : MessageFilter, ISerializationFilter
    {
        private readonly Dictionary<Type, Delegate> serializers;
        private readonly IBuffer buffer;

        internal SerializationFilter(IBuffer buffer, ISender sender) : base(sender)
        {
            this.buffer = buffer ?? throw new ArgumentNullException(nameof(buffer));
            serializers = new();
        }

        public void AddSerializer<T>(Action<IBufferWriter, T> serializer)
            => serializers.Add(typeof(T), serializer);

        public void RemoveSerializer<T>() => serializers.Remove(typeof(T));

        protected override bool HandleMessage<T>(T message)
        {
            if (serializers.ContainsKey(typeof(T)))
            {
                var writer = (Action<IBufferWriter, T>)serializers[typeof(T)];
                writer.Invoke(buffer.Writer, message);
                Sender.Send(new BufferUpdatedEvent(BufferType.Transmit, buffer.Reader));
                
                return true;
            }

            return false;
        }        

        protected override async Task<bool> HandleMessageAsync<T>(T message, CancellationToken cancellationToken)
        {
            if (serializers.ContainsKey(typeof(T)))
            {
                var writer = (Action<IBufferWriter, T>)serializers[typeof(T)];
                writer.Invoke(buffer.Writer, message);
                await Sender.SendAsync(new BufferUpdatedEvent(BufferType.Transmit, buffer.Reader), cancellationToken);
                
                return true;
            }

            return false;
        }
    }

    internal struct BufferUpdatedEvent
    {
        public BufferUpdatedEvent(BufferType bufferType, IBufferReader bufferReader)
            => (BufferType, BufferReader) = (bufferType, bufferReader);

        public BufferType BufferType { get; }

        public IBufferReader BufferReader { get; }
    }

    internal enum BufferType
    {
        Receive,
        Transmit
    }

    public interface ISerializationFilter
    {
        void AddSerializer<T>(Action<IBufferWriter, T> serializer);

        void RemoveSerializer<T>();
    }
}