using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RpgGame.Engine.Configuration
{
    public static class ConfigLoader
    {
        public static GameConfig LoadConfig(string filepath)
        {
            if(!File.Exists(filepath))
            {
                throw new FileNotFoundException($"Config file not found: {filepath}");
            }
            string jsonString = File.ReadAllText(filepath);
            return JsonSerializer.Deserialize<GameConfig>(jsonString);
        }
    }
}
