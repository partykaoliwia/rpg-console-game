using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Items.Weapons;

namespace RpgGame.Items
{
    public class Dory : MagicWeapon
    {
        public override string Symbol => "/";
        public override string Name => "Dory";
        public Dory(int damage) : base(damage) { }
    }
}
