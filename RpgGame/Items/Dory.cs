using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Items
{
    public class Dory : Weapon
    {
        public override string Symbol => "/";
        public override string Name => "Dory";
        public Dory() : base(15) { }
    }
}
