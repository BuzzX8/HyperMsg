using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    internal class SerializationFilter : MessageFilter, ISerializationFilter
    {
        private readonly Dictionary<Type, Delegate> writers;
        private readonly IBuffer buffer;

        internal SerializationFilter(IBuffer buffer, ISender sender) : base(sender)
        {
            this.buffer = buffer ?? throw new ArgumentNullException(nameof(buffer));
            writers = new();
        }

        public void AddSerializer<T>(Action<IBufferWriter, T> serializer)
            => writers.Add(typeof(T), serializer);

        public void RemoveSerializer<T>() => writers.Remove(typeof(T));

        protected override bool HandleMessage<T>(T message)
        {
            if (writers.ContainsKey(typeof(T)))
            {
                var writer = (Action<IBufferWriter, T>)writers[typeof(T)];
                writer.Invoke(buffer.Writer, message);
                buffer.Flush();
                
                return true;
            }

            return false;
        }        

        protected override async Task<bool> HandleMessageAsync<T>(T message, CancellationToken cancellationToken)
        {
            if (writers.ContainsKey(typeof(T)))
            {
                var writer = (Action<IBufferWriter, T>)writers[typeof(T)];
                writer.Invoke(buffer.Writer, message);
                await buffer.FlushAsync(cancellationToken);
                
                return true;
            }

            return false;
        }
    }

    public interface ISerializationFilter
    {
        void AddSerializer<T>(Action<IBufferWriter, T> serializer);

        void RemoveSerializer<T>();
    }
}