using HyperMsg.Messages;

namespace HyperMsg
{
    public abstract class TopicMessageFilter<T> : MessageFilter
    {
        public TopicMessageFilter(ISender messageSender) : base(messageSender)
        { }

        protected override bool HandleMessage<TMessage>(ref TMessage message)
        {
            if (message is TopicMessage<T> topicMessage)
            {
                var msg = topicMessage.Message;
                return HandleTopicMessage(topicMessage.TopicId, ref msg);
            }

            return false;
        }

        protected abstract bool HandleTopicMessage(object topicId, ref T message);
    }
}
