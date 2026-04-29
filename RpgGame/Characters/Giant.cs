using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Engine.Logging;
using RpgGame.Engine.Observers;

namespace RpgGame.Characters
{
    public class Giant : Enemy
    {
        // gatunek tchorzliwy
        public Giant(int health, int strength, int armor, string symbol, Color itemColor, SpeciesPublisher species, SoundPublisher sound)
           : base(health, strength, armor, symbol, itemColor,species,sound,"Giant") 
        { 
            MoveEveryMs = 1500; 
        }
        public override void UpdateOnSound(int sourceX, int sourceY, int range)
        {
            Logger.Instance.Log($"[{Name} ({X},{Y})] Heard noise emitted from {sourceX},{sourceY}. Distance from source: {range}");
        }
        public override void UpdateOnDeath()
        {
            Strength = Math.Max(1,(int)(0.33 * Strength));

        }
    }
}
