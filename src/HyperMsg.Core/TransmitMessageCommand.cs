﻿namespace HyperMsg
{
    internal struct TransmitMessageCommand<T>
    {
        public TransmitMessageCommand(T message) => Message = message;

        internal T Message { get; }
    }
}