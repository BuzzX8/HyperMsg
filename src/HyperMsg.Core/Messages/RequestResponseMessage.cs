namespace HyperMsg.Messages
{
    internal class RequestResponseMessage<TRequest, TResponse>
    {
        internal RequestResponseMessage(TRequest request) => Request = request;

        internal TRequest Request { get; }

        internal TResponse Response { get; set; }
    }
}
