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
        ISender Sender { get; }

        /// <summary>
        /// Message handlers registry.
        /// </summary>
        IHandlersRegistry HandlersRegistry { get; }
    }
}
