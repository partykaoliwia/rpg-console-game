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

        public override PlayerStats Bonuses
        {
            get
            {
                var b = _decorated.Bonuses;
                b.Luck += luck;
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
