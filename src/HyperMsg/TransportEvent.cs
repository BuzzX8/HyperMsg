namespace HyperMsg
{
    /// <summary>
    /// Messages which should be emited by transport layer in order to indicate certain events.
    /// </summary>
    public enum TransportEvent
    {
        Opening,
        Opened,
        Closing,
        Closed
    }
}