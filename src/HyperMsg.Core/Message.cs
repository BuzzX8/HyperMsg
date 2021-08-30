namespace HyperMsg
{
    internal struct Message<THeader, TBody>
    {
        internal Message(THeader header, TBody body) => (Header, Body) = (header, body);

        public THeader Header { get; }

        public TBody Body { get; }
    }
}