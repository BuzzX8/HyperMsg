namespace HyperMsg.Messages
{
    internal struct TopicMessage<T>
    {
        public TopicMessage(object topicId, T message, IMessageSender messageSender) => 
            (TopicId, Message, MessageSender) = (topicId, message, messageSender);

        public object TopicId { get; }

        public T Message { get; }

        public IMessageSender MessageSender { get; }
    }
}
