﻿using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Transciever
{
    public class MessageBuffer<T> : IMessageBuffer<T>
    {
        private readonly ByteBufferWriter writer;
        private readonly ISender<ReadOnlySequence<byte>> sender;
        private readonly SerializeAction<T> serializeAction;

        public MessageBuffer(IMemoryOwner<byte> memoryOwner, ISender<ReadOnlySequence<byte>> sender, SerializeAction<T> serializeAction)
        {
            writer = new ByteBufferWriter(memoryOwner);
            this.serializeAction = serializeAction ?? throw new ArgumentNullException(nameof(serializeAction));
        }

        public void Flush()
        {
            sender.Send(new ReadOnlySequence<byte>(writer.CommitedMemory));
            writer.Reset();
        }

        public async Task FlushAsync(CancellationToken token = default)
        {
            await sender.SendAsync(new ReadOnlySequence<byte>(writer.CommitedMemory), token);
            writer.Reset();
        }

        public void Write(T message) => serializeAction.Invoke(writer, message);
    }
}
