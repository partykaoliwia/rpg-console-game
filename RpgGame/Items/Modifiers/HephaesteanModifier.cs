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

        public override PlayerStats Bonuses
        {
            get
            {
                var b = _decorated.Bonuses;
                b.Dexterity += dexterity;
                return b;
            }
        }

        public override CombatStats Accept(IAttackVisitor visitor, Player player)
        {
            var stats = base.Accept(visitor, player);
            stats.Damage = (int)Math.Round(stats.Damage * damage);
            return stats;
        }
    }
}
