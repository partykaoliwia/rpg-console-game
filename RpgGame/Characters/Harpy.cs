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
    public class Harpy : Enemy
    {
        // gatunek agresywny
        public Harpy(int health, int strength, int armor, string symbol, Color itemColor, SpeciesPublisher species, SoundPublisher sound)
            : base(health, strength,armor,symbol,itemColor,species,sound,"Harpy") 
        {
            MoveEveryMs = 400;
        }
        public override void UpdateOnSound(int sourceX, int sourceY, int distance)
        {
            Logger.Instance.Log($"{Name} at ({X}, {Y}) detected noise from source ({sourceX}, {sourceY}). Distance: {distance}.");
        }
        public override void UpdateOnDeath()
        {
            Strength = 2 * Strength;
            
        }
    }
}
