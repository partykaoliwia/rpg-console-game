using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Engine.Logging
{
    public class MemoryLogWriter : ILogWriter
    {
        private readonly List<string> _logs = new();

        public void WriteLog(string message)
        {
            _logs.Add(message);
        }

        public IReadOnlyList<string> GetAllLogs() => _logs;
    }
}
