using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Characters;
using RpgGame.Combat;

namespace RpgGame.Items.Modifiers
{
    internal class HephaesteanModifier : ItemDecorator
    {
        // Damage +50%
        // Dexterity -15
        private double damage = 1.5;
        private int dexterity = -15;
        public HephaesteanModifier(IItem decorated) : base(decorated) { }
        public override string Name => _decorated.Name + "(Hephaestean)";

        public override void EquipToLeftHand(Player player)
        {
            base.EquipToLeftHand(player);
            player.Dexterity += dexterity;
        }
        public override void EquipToRightHand(Player player)
        {
            base.EquipToRightHand(player);
            player.Dexterity += dexterity;

        }
        public override void Unequip(Player player)
        {
            base.Unequip(player);
            player.Dexterity -= dexterity;

        }
        public override CombatStats Accept(IAttackVisitor visitor, Player player)
        {
            var stats = base.Accept(visitor, player);
            stats.Damage = (int)Math.Round(stats.Damage * damage);
            return stats;
        }
    }
}
