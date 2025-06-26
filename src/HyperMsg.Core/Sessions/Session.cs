using System.Collections.Concurrent;

namespace HyperMsg.Sessions;

public class Session : ISession
{
    public string Id { get; }
    public SessionState State { get; private set; }
    public IDictionary<string, object> Metadata { get; }
    public DateTime CreatedAt { get; }
    public DateTime? LastActivity { get; private set; }

    public Session(string id)
    {
        Id = id;
        State = SessionState.Active;
        Metadata = new ConcurrentDictionary<string, object>();
        CreatedAt = DateTime.UtcNow;
        LastActivity = CreatedAt;
    }

    public void Close()
    {
        State = SessionState.Closed;
        LastActivity = DateTime.UtcNow;
    }
}
