using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Characters;

namespace RpgGame.Items
{
    public class Labrys : Weapon
    {
        //oburęczna broń
        public override char Symbol => 'T';
        public override string Name => "Labrys";
        public Labrys() : base(35) { }
        public override void EquipToLeftHand(Player player)
        {
            player.EquipToBothHands(this);
        }
        public override void EquipToRightHand(Player player)
        {
            player.EquipToBothHands(this);
        }
    }
}
