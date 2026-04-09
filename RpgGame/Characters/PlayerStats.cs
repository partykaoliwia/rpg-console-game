using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Characters
{
    public struct PlayerStats
    {
        public int Strength { get; set; }
        public int Health { get; set; }
        public int Luck { get; set; }
        public int Aggression { get; set; }
        public int Wisdom { get; set; }
        public int Dexterity { get; set; }
        public static PlayerStats operator +(PlayerStats a, PlayerStats b) => new PlayerStats
        {
                Strength = a.Strength + b.Strength,
                Health = a.Health + b.Health,
                Luck = a.Luck + b.Luck,
                Aggression = a.Aggression + b.Aggression,
                Wisdom = a.Wisdom + b.Wisdom,
                Dexterity = a.Dexterity + b.Dexterity
        };
    }
}
