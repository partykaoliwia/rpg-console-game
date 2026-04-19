using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Engine.Logging
{
    public class Logger
    {
        private static readonly Lazy<Logger> _lazy  = new Lazy<Logger>(() => new Logger());
        public static Logger Instance => _lazy.Value;
        private readonly List<ILogWriter> _logWriters = new();
        private MemoryLogWriter _memoryWriter;
        private Logger() { }
        public void AddWriter(ILogWriter logWriter)
        {
            _logWriters.Add(logWriter);
        }
        public void SetMemoryWriter(MemoryLogWriter memoryWriter)
        {
            _memoryWriter = memoryWriter;
            AddWriter(memoryWriter);
        }
        public void Log(string message)
        {
            foreach (var writer in _logWriters)
            {
                writer.WriteLog(message);
            }
        }
        public IReadOnlyList<string> GetAllLogs()
        {
            return _memoryWriter?.GetAllLogs() ?? new List<string>();
        }
        public IReadOnlyList<string> GetRecentLogs(int count)
        {
            var allLogs = GetAllLogs();
            return allLogs.Skip(Math.Max(0, allLogs.Count - count)).ToList();
        }
    }
}
