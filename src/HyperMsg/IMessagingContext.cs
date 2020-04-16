namespace HyperMsg
{
    /// <summary>
    /// Represents context for message exchange.
    /// </summary>
    public interface IMessagingContext
    {
        /// <summary>
        /// Message sender.
        /// </summary>
        IMessageSender Sender { get; }

        /// <summary>
        /// Message observer.
        /// </summary>
        IMessageObservable Observable { get; }
    }
}
