namespace HyperMsg
{
    public struct DeserializationResult<T>
    {
        public DeserializationResult(int bytesConsumed, T message)
        {
            BytesConsumed = bytesConsumed;
            Message = message;
        }

        public int BytesConsumed { get; }

        public T Message { get; }
    }
}
