namespace HyperMsg.Sessions;

/// <summary>
/// Represents a communication session, which may encapsulate state, metadata, and lifecycle information for a logical connection or conversation.
/// </summary>
public interface ISession
{
    /// <summary>
    /// Gets the unique identifier for the session.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Gets the current state of the session.
    /// </summary>
    SessionState State { get; }

    /// <summary>
    /// Gets a dictionary for storing arbitrary session metadata.
    /// </summary>
    IDictionary<string, object> Metadata { get; }

    /// <summary>
    /// Gets the UTC timestamp when the session was created.
    /// </summary>
    DateTime CreatedAt { get; }

    /// <summary>
    /// Gets the UTC timestamp of the last activity in the session, or null if no activity has occurred since creation.
    /// </summary>
    DateTime? LastActivity { get; }

    /// <summary>
    /// Closes the session and updates its state and last activity timestamp.
    /// </summary>
    void Close();
}

/// <summary>
/// Represents the possible states of a session.
/// </summary>
public enum SessionState
{
    /// <summary>
    /// The session is active and available for communication.
    /// </summary>
    Active,
    /// <summary>
    /// The session has been closed and is no longer available.
    /// </summary>
    Closed,
    /// <summary>
    /// The session has expired due to timeout or inactivity.
    /// </summary>
    Expired
}
