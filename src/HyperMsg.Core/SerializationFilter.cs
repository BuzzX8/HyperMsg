using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    internal class SerializationFilter : ISender, ISerializationFilter
    {
        private readonly Dictionary<Type, Delegate> writers;
        private readonly IBuffer buffer;
        private readonly ISender sender;

        internal SerializationFilter(IBuffer buffer, ISender sender)
        {
            this.buffer = buffer ?? throw new ArgumentNullException(nameof(buffer));
            this.sender = sender ?? throw new ArgumentNullException(nameof(sender));
            writers = new();
        }

        public void AddSerializer<T>(Action<IBufferWriter, T> serializer)
            => writers.Add(typeof(T), serializer);

        public void RemoveSerializer<T>() => writers.Remove(typeof(T));

        public void Send<T>(T message)
        {
            if (writers.ContainsKey(typeof(T)))
            {
                var writer = (Action<IBufferWriter, T>)writers[typeof(T)];
                writer.Invoke(buffer.Writer, message);
                buffer.Flush();
                
                return;
            }

            sender.Send(message);
        }

        public Task SendAsync<T>(T message, CancellationToken cancellationToken)
        {
            if (writers.ContainsKey(typeof(T)))
            {
                var writer = (Action<IBufferWriter, T>)writers[typeof(T)];
                writer.Invoke(buffer.Writer, message);

                return buffer.FlushAsync(cancellationToken);
            }

            return sender.SendAsync(message, cancellationToken);
        }
    }

    public interface ISerializationFilter
    {
        void AddSerializer<T>(Action<IBufferWriter, T> serializer);

        void RemoveSerializer<T>();
    }
}