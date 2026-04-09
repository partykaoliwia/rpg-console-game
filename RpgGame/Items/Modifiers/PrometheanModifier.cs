using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Characters;
using RpgGame.Combat;

namespace RpgGame.Items.Modifiers
{
    internal class PrometheanModifier : ItemDecorator
    {
        // Wisdom +15
        // Health -10
        private int wisdom = 15;
        private int health = -10;
        public PrometheanModifier(IItem decorated) : base(decorated) { }
        public override string Name => _decorated.Name + "(Promethean)";

        public override PlayerStats Bonuses
        {
            get
            {
                var b = _decorated.Bonuses;
                b.Wisdom += wisdom;
                return b;
            }
        }

        public override CombatStats Accept(IAttackVisitor visitor, Player player)
        {
            return base.Accept(visitor,player);
        }
    }
}
