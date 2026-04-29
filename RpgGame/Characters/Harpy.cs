using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Engine.Observers;

namespace RpgGame.Characters
{
    public class Harpy : Enemy
    {
        // gatunek agresywny
        public Harpy(int health, int strength, int armor, string symbol, Color itemColor, SpeciesPublisher species)
            : base(health, strength,armor,symbol,itemColor,species,"Harpy") 
        {
            MoveEveryMs = 600;
        }
        public override void UpdateOnSound(int sourceX, int sourceY, int range)
        {
        }
        public override void UpdateOnDeath()
        {
            Strength = 2 * Strength;
            
        }
    }
}
