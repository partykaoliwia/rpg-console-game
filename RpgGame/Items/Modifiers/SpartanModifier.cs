using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Characters;
using RpgGame.Combat;

namespace RpgGame.Items.Modifiers
{
    public class SpartanModifier : ItemDecorator
    {
        // Strength +10
        // Luck -5
        private int strength = 10;
        private int luck = -5;
        public SpartanModifier(IItem decorated) : base(decorated) { }
        public override string Name => _decorated.Name + "(Spartan)";
        public override PlayerStats Bonuses
        {
            get
            {
                var b = _decorated.Bonuses;
                b.Strength += strength;
                b.Luck += luck;
                return b;
            }
        }

        public override CombatStats Accept(IAttackVisitor visitor, Player player)
        {
            return base.Accept(visitor, player);
        }
    }
}
