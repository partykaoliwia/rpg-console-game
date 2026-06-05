namespace RpgGame.Logging;

public sealed class MemoryLogWriter : ILogWriter
{
    private readonly object _syncRoot = new();
    private readonly List<string> _logs = new();

    public void WriteLog(string message)
    {
        lock (_syncRoot)
        {
            _logs.Add(message);
        }
    }

    public IReadOnlyList<string> GetAllLogs()
    {
        lock (_syncRoot)
        {
            return _logs.ToList();
        }
    }

    public IReadOnlyList<string> GetRecentLogs(int count)
    {
        lock (_syncRoot)
        {
            return _logs.Skip(Math.Max(0, _logs.Count - count)).ToList();
        }
    }
}
