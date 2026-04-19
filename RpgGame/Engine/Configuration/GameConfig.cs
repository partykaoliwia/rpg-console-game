using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Engine.Configuration
{
    public class GameConfig
    {
        public string PlayerName { get; set; } = "Hero";
        public string Theme { get; set; } = "Labyrinth";
        public string LogDirectory { get; set; } = "Logs";
    }
}
