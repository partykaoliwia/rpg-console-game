namespace RpgGame.Logging;

public sealed class Logger
{
    private static readonly Lazy<Logger> Lazy = new(() => new Logger());
    private readonly object _syncRoot = new();
    private readonly List<ILogWriter> _writers = new();
    private MemoryLogWriter? _memoryWriter;

    private Logger()
    {
    }

    public static Logger Instance => Lazy.Value;

    public void AddWriter(ILogWriter writer)
    {
        lock (_syncRoot)
        {
            _writers.Add(writer);
        }
    }

    public void SetMemoryWriter(MemoryLogWriter writer)
    {
        lock (_syncRoot)
        {
            _memoryWriter = writer;
            _writers.Add(writer);
        }
    }

    public void Log(string message)
    {
        List<ILogWriter> writers;
        lock (_syncRoot)
        {
            writers = _writers.ToList();
        }

        foreach (var writer in writers)
        {
            writer.WriteLog(message);
        }
    }

    public IReadOnlyList<string> GetAllLogs()
    {
        MemoryLogWriter? memoryWriter;
        lock (_syncRoot)
        {
            memoryWriter = _memoryWriter;
        }

        return memoryWriter?.GetAllLogs() ?? Array.Empty<string>();
    }

    public IReadOnlyList<string> GetRecentLogs(int count)
    {
        MemoryLogWriter? memoryWriter;
        lock (_syncRoot)
        {
            memoryWriter = _memoryWriter;
        }

        return memoryWriter?.GetRecentLogs(count) ?? Array.Empty<string>();
    }
}
