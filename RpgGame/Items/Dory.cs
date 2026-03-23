using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Items
{
    public class Dory : Weapon
    {
        public override char Symbol => '/';
        public override string Name => "Iron Sword";
        public Dory() : base(15) { }
    }
}
