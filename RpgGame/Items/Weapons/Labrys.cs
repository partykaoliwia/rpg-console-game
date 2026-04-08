using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Characters;
using RpgGame.Items.Weapons;

namespace RpgGame.Items
{
    public class Labrys : HeavyWeapon
    {
        //oburęczna broń
        public override string Symbol => "T";
        public override string Name => "Labrys";
        public Labrys(int damage) : base(damage) { }
        public override void EquipToLeftHand(Player player, IItem instanceToEquip = null)
        {
            player.EquipToBothHands(instanceToEquip ?? this);
        }
        public override void EquipToRightHand(Player player, IItem instanceToEquip = null)
        {
            player.EquipToBothHands(instanceToEquip ?? this);
        }
    }
}
