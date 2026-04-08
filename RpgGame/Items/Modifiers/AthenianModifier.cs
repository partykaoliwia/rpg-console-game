using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Characters;
using RpgGame.Combat;

namespace RpgGame.Items.Modifiers
{
    internal class AthenianModifier : ItemDecorator
    {
        // Wisdom +15
        // Damage +20%
        private int wisdom = 15;
        private double damage = 1.2;
        public AthenianModifier(IItem decorated) : base(decorated) { }
        public override string Name => _decorated.Name + "(Athenian)";

        public override void EquipToLeftHand(Player player, IItem instanceToEquip = null)
        {
            base.EquipToLeftHand(player, instanceToEquip ?? this);
            player.Wisdom += wisdom;
        }
        public override void EquipToRightHand(Player player, IItem instanceToEquip = null)
        {
            base.EquipToRightHand(player, instanceToEquip ?? this);
            player.Wisdom += wisdom;

        }
        public override void Unequip(Player player)
        {
            base.Unequip(player);
            player.Wisdom -= wisdom;

        }
        public override CombatStats Accept(IAttackVisitor visitor, Player player)
        {
            var stats = base.Accept(visitor, player);
            stats.Damage = (int)Math.Round(stats.Damage * damage);
            return stats;
        }
    }
}
