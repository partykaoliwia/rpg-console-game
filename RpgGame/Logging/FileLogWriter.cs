namespace RpgGame.Logging;

public sealed class FileLogWriter : ILogWriter
{
    private readonly object _syncRoot = new();
    private readonly string _filePath;

    public FileLogWriter(string filePath)
    {
        _filePath = filePath;
        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    public void WriteLog(string message)
    {
        lock (_syncRoot)
        {
            File.AppendAllText(_filePath, $"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");
        }
    }
}
