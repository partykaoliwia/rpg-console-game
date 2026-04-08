using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Characters;
using RpgGame.Combat;

namespace RpgGame.Items.Modifiers
{
    internal class MidasTouchModifier : ItemDecorator
    {
        // Damage -25%
        // Luck +20
        private int luck = 20;
        private double damage = 0.75;
        public MidasTouchModifier(IItem decorated) : base(decorated) { }
        public override string Name => _decorated.Name + "(Midas touch)";

        public override void EquipToLeftHand(Player player, IItem instanceToEquip = null)
        {
            base.EquipToLeftHand(player, instanceToEquip ?? this);
            player.Luck += luck;
        }
        public override void EquipToRightHand(Player player, IItem instanceToEquip = null)
        {
            base.EquipToRightHand(player, instanceToEquip ?? this);
            player.Luck += luck;

        }
        public override void Unequip(Player player)
        {
            base.Unequip(player);
            player.Luck -= luck;

        }
        public override CombatStats Accept(IAttackVisitor visitor, Player player)
        {
            var stats = base.Accept(visitor, player);
            stats.Damage = (int)Math.Round(stats.Damage * damage);
            return stats;
        }
    }
}
