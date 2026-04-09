using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Characters;
using RpgGame.Combat;
using static System.Net.Mime.MediaTypeNames;

namespace RpgGame.Items.Modifiers
{
    internal class AchillesBaneModifier : ItemDecorator
    {
        // Defense +100%
        // Luck -15
        private int luck = -15;
        private double defense = 2;
        public AchillesBaneModifier(IItem decorated) : base(decorated) { }
        public override string Name => _decorated.Name + "(Achilles bane)";
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
            stats.Defense = (int)Math.Round(stats.Defense * defense);
            return stats;
        }
    }
}
