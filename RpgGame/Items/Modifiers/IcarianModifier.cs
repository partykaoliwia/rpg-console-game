using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Characters;
using RpgGame.Combat;

namespace RpgGame.Items.Modifiers
{
    internal class IcarianModifier : ItemDecorator
    {
        // Damage +30%
        // Dexterity +10
        // Luck -10
        // Defense -15%
        private double damage = 1.3;
        private int dexterity = 10;
        private double defense = 0.85;
        private int luck = -10;
        public IcarianModifier(IItem decorated) : base(decorated) { }
        public override string Name => _decorated.Name + "(Icarian)";

        public override void EquipToLeftHand(Player player)
        {
            base.EquipToLeftHand(player);
            player.Luck += luck;
            player.Dexterity += dexterity;
        }
        public override void EquipToRightHand(Player player)
        {
            base.EquipToRightHand(player);
            player.Luck += luck;
            player.Dexterity += dexterity;

        }
        public override void Unequip(Player player)
        {
            base.Unequip(player);
            player.Luck -= luck;
            player.Dexterity -= dexterity;

        }
        public override CombatStats Accept(IAttackVisitor visitor, Player player)
        {
            var stats = base.Accept(visitor, player);
            stats.Defense = (int)Math.Round(stats.Defense * defense);
            stats.Damage = (int)Math.Round(stats.Damage * damage);
            return stats;
        }
    }
}
