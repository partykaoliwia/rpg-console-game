using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Characters;
using RpgGame.Combat;

namespace RpgGame.Items.Modifiers
{
    internal class HydrasVenomModifier : ItemDecorator
    {
        // Damage +50%
        // Health -10
        private int health = -10;
        private double damage = 1.5;
        public HydrasVenomModifier(IItem decorated) : base(decorated) { }
        public override string Name => _decorated.Name + "(Hydra's venom)";

        public override PlayerStats Bonuses
        {
            get
            {
                var b = _decorated.Bonuses;
                b.Health += health;
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
