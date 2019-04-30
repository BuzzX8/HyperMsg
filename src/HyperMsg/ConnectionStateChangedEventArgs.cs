using System;

namespace HyperMsg
{
    public class ConnectionStateChangedEventArgs : EventArgs
    {
        public ConnectionStateChangedEventArgs(ConnectionState state)
        {
            State = state;
        }

        public ConnectionState State { get; }
    }
}