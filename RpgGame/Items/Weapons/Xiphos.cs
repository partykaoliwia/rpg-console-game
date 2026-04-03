using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Items.Weapons;

namespace RpgGame.Items
{
    public class Xiphos : LightWeapon
    {
        public override string Symbol => "†";
        public override string Name => "Xiphos";
        public Xiphos(int damage) : base(damage) { }
    }
}
