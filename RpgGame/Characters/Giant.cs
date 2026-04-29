using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Engine.Observers;

namespace RpgGame.Characters
{
    public class Giant : Enemy
    {
        // gatunek tchorzliwy
        public Giant(int health, int strength, int armor, string symbol, Color itemColor, SpeciesPublisher species)
           : base(health, strength, armor, symbol, itemColor,species,"Giant") 
        { 
            MoveEveryMs = 1500; 
        }
        public override void UpdateOnSound(int sourceX, int sourceY, int range)
        {
            // TODO
        }
        public override void UpdateOnDeath()
        {
            Strength = Math.Max(1,(int)(0.33 * Strength));

        }
    }
}
