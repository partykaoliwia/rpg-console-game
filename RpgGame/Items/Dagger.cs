using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Items
{
    public class Dagger : Weapon
    {
        public override char Symbol => '-';
        public override string Name => "Fire Dagger";
        public Dagger() : base(8) { }
    }
}
