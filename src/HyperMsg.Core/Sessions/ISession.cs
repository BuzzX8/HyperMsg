namespace HyperMsg.Sessions;

/// <summary>
/// Represents a communication session.
/// </summary>
public interface ISession
{
    string Id { get; }
    SessionState State { get; }
    IDictionary<string, object> Metadata { get; }
    DateTime CreatedAt { get; }
    DateTime? LastActivity { get; }
    void Close();
}

public enum SessionState
{
    Active,
    Closed,
    Expired
}
