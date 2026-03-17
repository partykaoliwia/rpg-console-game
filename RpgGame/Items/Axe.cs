using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Characters;

namespace RpgGame.Items
{
    public class Axe : Weapon
    {
        public override char Symbol => 'T';
        public override string Name => "Two Handed Axe";
        public Axe() : base(35) { }
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
