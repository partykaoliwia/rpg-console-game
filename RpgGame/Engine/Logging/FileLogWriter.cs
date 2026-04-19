using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Engine.Logging
{
    public class FileLogWriter : ILogWriter
    {
        private readonly string _filePath;

        public FileLogWriter(string filePath)
        {
            _filePath = filePath;
        }

        public void WriteLog(string message)
        {
            // Dopisywanie do pliku. Warto dodać timestamp!
            File.AppendAllText(_filePath, $"[{DateTime.Now:HH:mm:ss}] {message}\n");
        }
    }
}
